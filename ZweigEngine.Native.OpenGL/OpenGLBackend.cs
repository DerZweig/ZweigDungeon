using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Common.Utility.Exceptions;
using ZweigEngine.Native.OpenGL.Constants;
using ZweigEngine.Native.OpenGL.Interfaces;
using ZweigEngine.Native.OpenGL.Prototypes;
using ZweigEngine.Native.OpenGL.Renderer;

namespace ZweigEngine.Native.OpenGL;

public class OpenGLBackend : IDisposable, IVideoBackend
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

	private readonly PfnEnableDelegate        glEnable;
	private readonly PfnDisableDelegate       glDisable;
	private readonly PfnClearColorDelegate    glClearColor;
	private readonly PfnClearDepthDelegate    glClearDepth;
	private readonly PfnClearDelegate         glClear;
	private readonly PfnBlendColorDelegate    glBlendColor;
	private readonly PfnBlendFunctionDelegate glBlendFunc;
	private readonly PfnDepthFunctionDelegate glDepthFunc;
	private readonly PfnCullFaceDelegate      glCullFace;
	private readonly PfnFrontFaceDelegate     glFrontFace;
	private readonly PfnGetErrorDelegate      glGetError;
	private readonly PfnDepthMaskDelegate     glDepthMask;
	private readonly PfnColorMaskDelegate     glColorMask;
	private readonly PfnViewportDelegate      glViewport;
	private readonly PfnScissorDelegate       glScissor;

	// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
	// ReSharper restore InconsistentNaming

	private readonly OpenGLSpriteShader   m_spriteShader;
	private readonly OpenGLSpriteModel    m_spriteModel;
	private readonly OpenGLSpriteTextures m_spriteTextures;

	public OpenGLBackend(IOpenGLSurface surface)
	{
		surface.LoadFunction(nameof(glEnable), out glEnable);
		surface.LoadFunction(nameof(glDisable), out glDisable);
		surface.LoadFunction(nameof(glClearColor), out glClearColor);
		surface.LoadFunction(nameof(glClearDepth), out glClearDepth);
		surface.LoadFunction(nameof(glClear), out glClear);
		surface.LoadFunction(nameof(glBlendColor), out glBlendColor);
		surface.LoadFunction(nameof(glBlendFunc), out glBlendFunc);
		surface.LoadFunction(nameof(glDepthMask), out glDepthMask);
		surface.LoadFunction(nameof(glDepthFunc), out glDepthFunc);
		surface.LoadFunction(nameof(glCullFace), out glCullFace);
		surface.LoadFunction(nameof(glFrontFace), out glFrontFace);
		surface.LoadFunction(nameof(glGetError), out glGetError);
		surface.LoadFunction(nameof(glColorMask), out glColorMask);
		surface.LoadFunction(nameof(glViewport), out glViewport);
		surface.LoadFunction(nameof(glScissor), out glScissor);
		m_spriteShader   = new OpenGLSpriteShader(surface);
		m_spriteModel    = new OpenGLSpriteModel(surface);
		m_spriteTextures = new OpenGLSpriteTextures(surface);
	}

	private void ReleaseUnmanagedResources()
	{
		m_spriteModel.Dispose();
		m_spriteShader.Dispose();
		m_spriteTextures.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLBackend()
	{
		ReleaseUnmanagedResources();
	}
	
	
	public void BeginScene(in VideoRect viewport)
	{
		glViewport(viewport.Left, viewport.Top, viewport.Width, viewport.Height);
		glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
		glClearDepth(1.0);
		glClear(OpenGLClearMask.ColorBufferBit | OpenGLClearMask.DepthBufferBit);
		m_spriteShader.Begin(viewport.Width, viewport.Height);
		m_spriteModel.Begin();
	}

	public void FinishScene()
	{
		m_spriteModel.Flush();
		m_spriteModel.Finish();
		m_spriteShader.Finish();
	}


	public void SetBlendMode(VideoBlendMode mode)
	{
		switch (mode)
		{
			case VideoBlendMode.Default:
			case VideoBlendMode.Alpha:
				glBlendFunc(OpenGLBlendSourceFactor.SrcAlpha, OpenGLBlendDestinationFactor.OneMinusSrcAlpha);
				break;
			case VideoBlendMode.Additive:
				glBlendFunc(OpenGLBlendSourceFactor.SrcAlpha, OpenGLBlendDestinationFactor.One);
				break;
			case VideoBlendMode.Multiply:
				glBlendFunc(OpenGLBlendSourceFactor.Zero, OpenGLBlendDestinationFactor.SrcColor);
				break;
			case VideoBlendMode.Subtract:
				glBlendFunc(OpenGLBlendSourceFactor.Zero, OpenGLBlendDestinationFactor.OneMinusSrcColor);
				break;
			default:
				throw new EnumOutOfRangeException<VideoBlendMode>(mode);
		}
	}

	public void ClearScreen()
	{
		glClear(OpenGLClearMask.ColorBufferBit | OpenGLClearMask.DepthBufferBit);
	}

	public void FlushPending()
	{
		m_spriteModel.Flush();
	}

	public void CreateImage(ushort width, ushort height, IntPtr data, out uint name)
	{
		name = m_spriteTextures.Create(width, height, data);
	}

	public void DestroyImage(uint name)
	{
		m_spriteTextures.Release(name);
	}

	public void BindImage(uint? name)
	{
		m_spriteTextures.Bind(name);
	}

	public void UploadImage(uint name, ushort width, ushort height, IntPtr data)
	{
		m_spriteTextures.Upload(name, width, height, data);
	}

	public void DrawImage(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags)
	{
		m_spriteModel.Draw(dstRegion, srcRegion, tintColor, blitFlags);
	}
}
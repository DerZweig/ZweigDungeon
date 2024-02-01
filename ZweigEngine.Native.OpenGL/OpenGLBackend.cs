using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Common.Utility.Exceptions;
using ZweigEngine.Native.OpenGL.Constants;
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

	public OpenGLBackend(IVideoSurface surface)
	{
		var loader = (ICustomFunctionLoader)surface;
		loader.LoadFunction(nameof(glEnable), out glEnable);
		loader.LoadFunction(nameof(glDisable), out glDisable);
		loader.LoadFunction(nameof(glClearColor), out glClearColor);
		loader.LoadFunction(nameof(glClearDepth), out glClearDepth);
		loader.LoadFunction(nameof(glClear), out glClear);
		loader.LoadFunction(nameof(glBlendColor), out glBlendColor);
		loader.LoadFunction(nameof(glBlendFunc), out glBlendFunc);
		loader.LoadFunction(nameof(glDepthMask), out glDepthMask);
		loader.LoadFunction(nameof(glDepthFunc), out glDepthFunc);
		loader.LoadFunction(nameof(glCullFace), out glCullFace);
		loader.LoadFunction(nameof(glFrontFace), out glFrontFace);
		loader.LoadFunction(nameof(glGetError), out glGetError);
		loader.LoadFunction(nameof(glColorMask), out glColorMask);
		loader.LoadFunction(nameof(glViewport), out glViewport);
		loader.LoadFunction(nameof(glScissor), out glScissor);
		m_spriteShader   = new OpenGLSpriteShader(loader);
		m_spriteModel    = new OpenGLSpriteModel(loader);
		m_spriteTextures = new OpenGLSpriteTextures(loader);
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
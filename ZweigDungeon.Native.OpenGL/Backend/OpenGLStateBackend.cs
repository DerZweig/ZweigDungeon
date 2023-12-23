using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Backend;

internal sealed class OpenGLStateBackend : IDisposable
{
	// ReSharper disable InconsistentNaming
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
	// ReSharper restore InconsistentNaming

	public OpenGLStateBackend(ICustomFunctionLoader loader)
	{
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
	}
	
	private void ReleaseUnmanagedResources()
	{
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLStateBackend()
	{
		ReleaseUnmanagedResources();
	}
}
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Backend;

internal class OpenGLFrameBufferBackend : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnGenFramebuffersDelegate         glGenFramebuffers;
	private readonly PfnDeleteFramebuffersDelegate      glDeleteFramebuffers;
	private readonly PfnBindFramebufferDelegate         glBindFramebuffer;
	private readonly PfnCheckFramebufferStatusDelegate  glCheckFramebufferStatus;
	private readonly PfnFramebufferTexture2DDelegate    glFramebufferTexture2D;
	private readonly PfnFramebufferRenderbufferDelegate glFramebufferRenderbuffer;
	private readonly PfnGenRenderbuffersDelegate        glGenRenderbuffers;
	private readonly PfnDeleteRenderbuffersDelegate     glDeleteRenderbuffers;
	private readonly PfnBindRenderbufferDelegate        glBindRenderbuffer;
	private readonly PfnRenderbufferStorageDelegate     glRenderbufferStorage;
	// ReSharper restore InconsistentNaming

	public OpenGLFrameBufferBackend(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glGenFramebuffers), out glGenFramebuffers);
		loader.LoadFunction(nameof(glDeleteFramebuffers), out glDeleteFramebuffers);
		loader.LoadFunction(nameof(glBindFramebuffer), out glBindFramebuffer);
		loader.LoadFunction(nameof(glCheckFramebufferStatus), out glCheckFramebufferStatus);
		loader.LoadFunction(nameof(glFramebufferTexture2D), out glFramebufferTexture2D);
		loader.LoadFunction(nameof(glFramebufferRenderbuffer), out glFramebufferRenderbuffer);
		loader.LoadFunction(nameof(glGenRenderbuffers), out glGenRenderbuffers);
		loader.LoadFunction(nameof(glDeleteRenderbuffers), out glDeleteRenderbuffers);
		loader.LoadFunction(nameof(glBindRenderbuffer), out glBindRenderbuffer);
		loader.LoadFunction(nameof(glRenderbufferStorage), out glRenderbufferStorage);
	}

	private void ReleaseUnmanagedResources()
	{
		// TODO release unmanaged resources here
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLFrameBufferBackend()
	{
		ReleaseUnmanagedResources();
	}
}
namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLRenderBufferResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLRenderBufferResource(OpenGLContext context)
	{
		m_context = context;
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

	~OpenGLRenderBufferResource()
	{
		ReleaseUnmanagedResources();
	}
}
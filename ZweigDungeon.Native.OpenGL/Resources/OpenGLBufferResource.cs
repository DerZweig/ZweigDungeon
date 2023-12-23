namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLBufferResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLBufferResource(OpenGLContext context)
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

	~OpenGLBufferResource()
	{
		ReleaseUnmanagedResources();
	}
}
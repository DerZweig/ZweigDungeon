namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLArrayResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLArrayResource(OpenGLContext context)
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

	~OpenGLArrayResource()
	{
		ReleaseUnmanagedResources();
	}
}
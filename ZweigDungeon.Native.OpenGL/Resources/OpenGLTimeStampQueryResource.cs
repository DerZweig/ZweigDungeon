namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLTimeStampQueryResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLTimeStampQueryResource(OpenGLContext context)
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

	~OpenGLTimeStampQueryResource()
	{
		ReleaseUnmanagedResources();
	}
}
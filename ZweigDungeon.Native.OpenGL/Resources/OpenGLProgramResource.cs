namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLProgramResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLProgramResource(OpenGLContext context)
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

	~OpenGLProgramResource()
	{
		ReleaseUnmanagedResources();
	}
}
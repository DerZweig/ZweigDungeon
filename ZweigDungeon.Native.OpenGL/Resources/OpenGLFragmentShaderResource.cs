namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLFragmentShaderResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLFragmentShaderResource(OpenGLContext context)
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

	~OpenGLFragmentShaderResource()
	{
		ReleaseUnmanagedResources();
	}
}
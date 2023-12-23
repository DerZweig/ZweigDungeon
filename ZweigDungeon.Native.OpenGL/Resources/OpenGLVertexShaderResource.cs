namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLVertexShaderResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLVertexShaderResource(OpenGLContext context)
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

	~OpenGLVertexShaderResource()
	{
		ReleaseUnmanagedResources();
	}
}
namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLTexture2DResource : IDisposable
{
	private readonly OpenGLContext m_context;

	public OpenGLTexture2DResource(OpenGLContext context)
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

	~OpenGLTexture2DResource()
	{
		ReleaseUnmanagedResources();
	}
}
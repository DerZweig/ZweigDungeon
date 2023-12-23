namespace ZweigDungeon.Common.Services.Video.Resources;

public abstract class AbstractVideoResource : IDisposable
{
	private readonly VideoContext m_context;

	internal AbstractVideoResource(VideoContext context)
	{
		m_context = context;
	}
	
	private void ReleaseUnmanagedResources()
	{
		m_context.Delete(this);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~AbstractVideoResource()
	{
		ReleaseUnmanagedResources();
	}
}
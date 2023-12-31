using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class Game : IDisposable, IWindowListener
{
	private readonly IVideoContext m_video;
	private readonly IDisposable   m_subscription;

	public Game(MessageBus messageBus, IVideoContext video)
	{
		m_video        = video;
		m_subscription = messageBus.Subscribe<IWindowListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_subscription.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Game()
	{
		ReleaseUnmanagedResources();
	}

	public void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetMinimumSize(640, 480);
		window.Show();
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{

	}
}
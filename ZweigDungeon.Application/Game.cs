using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Common.Services.Video;

namespace ZweigDungeon.Application;

public class Game : IDisposable, IWindowListener
{
	private readonly VideoContext            m_video;
	private readonly IDisposable             m_subscription;
	private readonly CancellationTokenSource m_cancellationTokenSource;

	public Game(MessageBus messageBus, VideoContext video)
	{
		m_video                   = video;
		m_subscription            = messageBus.Subscribe<IWindowListener>(this);
		m_cancellationTokenSource = new CancellationTokenSource();
	}

	private void ReleaseUnmanagedResources()
	{
		m_cancellationTokenSource.Dispose();
	}

	private void Dispose(bool disposing)
	{
		ReleaseUnmanagedResources();
		if (disposing)
		{
			m_subscription.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Game()
	{
		Dispose(false);
	}

	public void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetMinimumSize(640, 480);
		window.Show();
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_cancellationTokenSource.Cancel();
	}

	public void WindowBeginFrame(IPlatformWindow window)
	{
		//todo update menu layout
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		//todo process current state
	}

	public void WindowFinishFrame(IPlatformWindow window)
	{
		//todo draw frame
	}
}
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Messages;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class App : IDisposable, IWindowListener
{
	private readonly IPlatformSynchronization m_synchronization;
	private readonly IDisposable              m_subscription;

	public App(MessageBus messageBus, IPlatformSynchronization synchronization)
	{
		m_synchronization = synchronization;
		m_subscription    = messageBus.Subscribe<IWindowListener>(this);
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

	~App()
	{
		ReleaseUnmanagedResources();
	}

	public async void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetStyle(true, true);
		window.SetMinimumSize(640, 480);
		window.Show();
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width    = window.GetViewportWidth();
		var height   = window.GetViewportHeight();
	}
}
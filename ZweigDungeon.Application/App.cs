using ZweigEngine.Common.Services.Interfaces.Platform;

namespace ZweigDungeon.Application;

public class App : IDisposable
{
	private readonly IPlatformSynchronization m_synchronization;
	private readonly IPlatformWindow          m_window;

	public App(IPlatformSynchronization synchronization, IPlatformWindow window)
	{
		m_synchronization  =  synchronization;
		m_window           =  window;
		m_window.OnCreated += HandleWindowCreated;
		m_window.OnClosing += HandleWindowClosing;
		m_window.OnUpdate  += HandleWindowUpdate;
	}

	private void ReleaseUnmanagedResources()
	{
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

	private void HandleWindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetStyle(true, true);
		window.SetMinimumSize(640, 480);
		window.Show();
	}

	private void HandleWindowClosing(IPlatformWindow window)
	{
	}

	private void HandleWindowUpdate(IPlatformWindow window)
	{
		var width  = window.GetViewportWidth();
		var height = window.GetViewportHeight();
	}
}
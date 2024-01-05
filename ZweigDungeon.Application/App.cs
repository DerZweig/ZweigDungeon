using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Platform.Messages;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class App : IDisposable, IWindowListener
{
	private readonly IMenuManager    m_menuManager;
	private readonly ILayoutManager  m_layoutManager;
	private readonly IDisposable     m_subscription;
	private          MenuDefinition? m_activeMenu;

	public App(MessageBus messageBus, IMenuManager menuManager, ILayoutManager layoutManager)
	{
		m_menuManager   = menuManager;
		m_layoutManager = layoutManager;
		m_subscription  = messageBus.Subscribe<IWindowListener>(this);
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

		m_activeMenu = await m_menuManager.LoadMenu("Menu/StartupMenu");
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width  = window.GetViewportWidth();
		var height = window.GetViewportHeight();

		if (m_activeMenu != null)
		{
			m_layoutManager.UpdateLayout(m_activeMenu, new VideoRect { Left = 0, Top = 0, Width = width, Height = height });
		}
	}
}
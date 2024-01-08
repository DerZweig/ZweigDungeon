using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Messages;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class App : IDisposable, IWindowListener
{
	private readonly IMenuRepository m_menuRepository;
	private readonly IMenuRenderer   m_menuRenderer;
	private readonly IDisposable     m_subscription;
	private          MenuDefinition? m_menuActive;

	public App(MessageBus messageBus, IMenuRepository menuRepository, IMenuRenderer menuRenderer)
	{
		m_menuRepository = menuRepository;
		m_menuRenderer   = menuRenderer;
		m_subscription   = messageBus.Subscribe<IWindowListener>(this);
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
		m_menuActive = await m_menuRepository.LoadMenu("Menu/StartupMenu");
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width    = window.GetViewportWidth();
		var height   = window.GetViewportHeight();
		var viewport = new VideoRect { Left = 0, Top = 0, Width = width, Height = height };

		if (m_menuActive != null)
		{
			m_menuRenderer.Draw(m_menuActive, viewport);
		}
	}
}
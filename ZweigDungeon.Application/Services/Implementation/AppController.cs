using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Video.Structures;

namespace ZweigDungeon.Application.Services.Implementation;

public class AppController : IDisposable, IAppController
{
	private readonly IPlatformWindow m_window;
	private readonly IGlobalAssets   m_assets;
	private readonly MenuController  m_menu;
	private readonly MapController   m_map;
	private readonly GlobalTimers    m_timers;

	public AppController(IPlatformWindow window, IGlobalAssets assets, MenuController menu, MapController map, GlobalTimers timers)
	{
		m_window = window;
		m_assets = assets;
		m_menu   = menu;
		m_map    = map;
		m_timers = timers;

		m_window.OnCreated += HandleWindowCreated;
		m_window.OnClosing += HandleWindowClosing;
		m_window.OnUpdate  += HandleWindowUpdate;
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.OnCreated -= HandleWindowCreated;
		m_window.OnClosing -= HandleWindowClosing;
		m_window.OnUpdate  -= HandleWindowUpdate;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~AppController()
	{
		ReleaseUnmanagedResources();
	}

	private void HandleWindowCreated(IPlatformWindow window)
	{
		window.SetStyle(WindowStyle.Windowed);
		window.SetTitle("My Application");
		window.SetMinimumSize(640, 480);
		m_timers.Reset();
	}

	private void HandleWindowClosing(IPlatformWindow window)
	{
	}

	private void HandleWindowUpdate(IPlatformWindow window)
	{
		if (!m_assets.IsLoaded())
		{
			return;
		}

		var width    = window.GetViewportWidth();
		var height   = window.GetViewportHeight();
		var viewport = new VideoRect { Left = 0, Top = 0, Width = width, Height = height };

		m_timers.Update();
		m_map.Update(viewport);
		m_menu.Update(viewport);

		m_map.Display(viewport);
		m_menu.Display(viewport);
	}
}
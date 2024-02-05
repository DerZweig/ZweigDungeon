using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.DirectX;
using ZweigEngine.Native.OpenGL;
using ZweigEngine.Native.OpenGL.Win32;

namespace ZweigDungeon.Application.Services.Implementation;

public class AppController : IDisposable, IAppController
{
	private readonly NativeLibraryLoader m_loader;
	private readonly IPlatformWindow     m_window;
	private readonly IPlatformVideo      m_video;
	private readonly IGlobalAssets       m_assets;
	private readonly MenuController      m_menu;
	private readonly MapController       m_map;
	private readonly GlobalTimers        m_timers;

	public AppController(NativeLibraryLoader loader, IPlatformWindow window, IPlatformVideo video, IGlobalAssets assets, MenuController menu, MapController map, GlobalTimers timers)
	{
		m_loader = loader;
		m_window = window;
		m_video  = video;
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
		//m_video.Configure(() => new Win32OpenGLSurface(m_loader, m_window, 3, 3), surf => new OpenGLBackend((Win32OpenGLSurface)surf));
		m_video.Configure(() => new D3D11Surface(m_loader, m_window), surf => new D3D11Backend((D3D11Surface)surf));
		
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
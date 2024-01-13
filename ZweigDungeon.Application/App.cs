using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application;

public class App : IDisposable
{
	private readonly IPlatformWindow m_window;
	private readonly IMenuController m_menuController;
	private readonly ILayoutManager  m_layout;
	private readonly IMenuRenderer   m_menuRenderer;

	public App(IPlatformWindow window, ILayoutManager layout,
	           IMenuController menuController, IMenuRenderer menuRenderer)
	{
		m_window           =  window;
		m_menuController   =  menuController;
		m_layout           =  layout;
		m_menuRenderer     =  menuRenderer;
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
		m_menuController.DisplayStartupMenu();
	}

	private void HandleWindowClosing(IPlatformWindow window)
	{
	}

	private void HandleWindowUpdate(IPlatformWindow window)
	{
		var width    = window.GetViewportWidth();
		var height   = window.GetViewportHeight();
		var viewport = new VideoRect { Left = 0, Top = 0, Width = width, Height = height };

		m_layout.Update(viewport);
		m_menuRenderer.Draw(viewport);
	}
}
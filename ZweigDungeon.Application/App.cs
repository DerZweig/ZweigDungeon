using System;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Repositories;

namespace ZweigDungeon.Application;

public class App : IDisposable
{
	private readonly IPlatformWindow     m_window;

	public App(IPlatformWindow window)
	{
		m_window           =  window;
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
	}

	private void HandleWindowClosing(IPlatformWindow window)
	{
	}

	private void HandleWindowUpdate(IPlatformWindow window)
	{
		var width    = window.GetViewportWidth();
		var height   = window.GetViewportHeight();
		var viewport = new VideoRect { Left = 0, Top = 0, Width = width, Height = height };
	}
}
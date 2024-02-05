using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;

namespace ZweigEngine.Native.Win32;

public class Win32Video : IDisposable, IPlatformVideo, IWin32WindowComponent
{
	private readonly Win32Window    m_window;
	private          IVideoSurface? m_surface;
	private          IVideoBackend? m_backend;

	public Win32Video(NativeLibraryLoader loader, Win32Window window)
	{
		m_window = window;
		m_window.AddComponent(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.RemoveComponent(this);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Win32Video()
	{
		ReleaseUnmanagedResources();
	}

	public event PlatformVideoActivateDelegate? OnActivated;
	public event PlatformVideoDelegate?         OnDeactivating;
	public event PlatformVideoDelegate?         OnBeginFrame;
	public event PlatformVideoDelegate?         OnFinishFrame;

	public void Configure(Func<IVideoSurface> surfaceFactory, Func<IVideoSurface, IVideoBackend> backendFactory)
	{
		ReleaseResources();
		m_surface = surfaceFactory();
		m_backend = backendFactory(m_surface);
		OnActivated?.Invoke(this, m_backend);
	}

	void IWin32WindowComponent.OnAttach()
	{
	}

	void IWin32WindowComponent.OnDetach()
	{
		ReleaseResources();
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		if (m_surface != null)
		{
			var width  = m_window.GetViewportWidth();
			var height = m_window.GetViewportHeight();
			if (width != m_surface.Width || height != m_surface.Height)
			{
				m_surface.Resize(width, height);
			}
		}

		if (m_backend != null)
		{
			OnBeginFrame?.Invoke(this);
		}
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
		if (m_backend != null)
		{
			OnFinishFrame?.Invoke(this);
		}

		m_surface?.Present();
	}

	void IWin32WindowComponent.OnMessage(long lTime, IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam)
	{
	}

	private void ReleaseResources()
	{
		if (m_backend != null)
		{
			OnDeactivating?.Invoke(this);
			(m_backend as IDisposable)?.Dispose();
			(m_surface as IDisposable)?.Dispose();
			m_backend = null;
			m_surface = null;
		}
	}
}
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;
using ZweigEngine.Native.Win32.Video;

namespace ZweigEngine.Native.Win32;

public class Win32Video : IDisposable, IPlatformVideo, IWin32WindowComponent
{
	private readonly Win32Window                      m_window;
	private readonly IReadOnlyList<IWin32VideoDriver> m_drivers;
	private          IWin32VideoSurface?              m_surface;
	private          IVideoBackend?                   m_backend;
	private          Action?                          m_currentConfig;
	private          Action?                          m_nextConfig;

	public Win32Video(NativeLibraryLoader loader, Win32Window window)
	{
		m_window = window;
		m_drivers = new IWin32VideoDriver[]
		{
			new Win32OpenGLVideoDriver(loader, m_window, 3, 3),
			new Win32Direct3DVideoDriver(loader, window, "DirectX 11")
		};

		m_window.AddComponent(this);
	}

	private void ReleaseUnmanagedResources()
	{
		try
		{
			m_window.RemoveComponent(this);
		}
		finally
		{
			foreach (var driver in m_drivers)
			{
				if (driver is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
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

	public IEnumerable<IVideoDriver> EnumerateDrivers()
	{
		return m_drivers;
	}

	public void ConfigureSurface(IVideoDriver driver, in VideoDeviceDescription deviceDescription, Func<IVideoSurface, IVideoBackend> backendFactory)
	{
		if (driver is IWin32VideoDriver win32Driver)
		{
			var desc = deviceDescription;
			m_nextConfig = () =>
			{
				ReleaseResources();

				m_surface = win32Driver.CreateSurface(desc);
				m_backend = backendFactory(m_surface);
				OnActivated?.Invoke(this, m_backend);
			};
		}
		else
		{
			throw new Exception("Attempted to configure surface for unknown video driver.");
		}
	}

	void IWin32WindowComponent.OnAttach()
	{
		ApplyConfigure();
	}

	void IWin32WindowComponent.OnDetach()
	{
		try
		{
			OnDeactivating?.Invoke(this);
		}
		finally
		{
			m_currentConfig = null;
			ReleaseResources();
		}
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		ApplyConfigure();
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

	private void ApplyConfigure()
	{
		if (m_nextConfig != m_currentConfig)
		{
			m_currentConfig = m_nextConfig;
			m_currentConfig?.Invoke();
		}
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
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.DirectX.Constants;
using ZweigEngine.Native.Win32.DirectX.D3D11;
using ZweigEngine.Native.Win32.DirectX.DXGI;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32;

public sealed class D3D11VideoDevice : IPlatformVideo, IDisposable, IWin32WindowComponent
{
	private readonly NativeLibraryLoader m_libraryLoader;
	private readonly Win32Window         m_window;
	
	private bool                m_presentError;
	private DXGIFactory?        m_factory;
	private D3D11Device?        m_device;
	private D3D11DeviceContext? m_context;
	private DXGISwapChain?      m_swapChain;
	private int                 m_width;
	private int                 m_height;

	public D3D11VideoDevice(NativeLibraryLoader libraryLoader, Win32Window window)
	{
		m_libraryLoader = libraryLoader;
		m_window        = window;
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

	~D3D11VideoDevice()
	{
		ReleaseUnmanagedResources();
	}

	public event PlatformVideoDeviceDelegate? OnActivated;
	public event PlatformVideoDeviceDelegate? OnDeactivating;
	public event PlatformVideoDeviceDelegate? OnBeginFrame;
	public event PlatformVideoDeviceDelegate? OnFinishFrame;

	public string GetDeviceName()     => "Direct3D 11";
	public int    GetViewportWidth()  => m_width;
	public int    GetViewportHeight() => m_height;

	void IWin32WindowComponent.OnAttach()
	{
		if (!m_window.IsAvailable() || m_device != null || m_context != null)
		{
			return;
		}

		if (!DXGIFactory.TryCreate(m_libraryLoader, out m_factory))
		{
			throw new Exception("Couldn't initialize DXGI Factory.");
		}

		if (!D3D11Device.TryCreate(m_libraryLoader, null, Direct3DDriverType.Hardware, out m_device, out m_context))
		{
			throw new Exception("Couldn't create D3D11 device & context.");
		}

		CreateSwapchain();
		m_width  = m_window.GetViewportWidth();
		m_height = m_window.GetViewportHeight();
	}

	void IWin32WindowComponent.OnDetach()
	{
		try
		{
			m_context?.Flush();
			m_context?.ClearState();
			OnDeactivating?.Invoke(this);
		}
		finally
		{
			m_swapChain?.Dispose();
			m_context?.Dispose();
			m_device?.Dispose();
			m_factory?.Dispose();
		}
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		
		var width  = m_window.GetViewportWidth();
		var height = m_window.GetViewportHeight();
		if (width != m_width || height != m_height)
		{
			ResizeSwapchain(width, height);
			m_width  = width;
			m_height = height;
		}
		
		OnBeginFrame?.Invoke(this);
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
		if (m_swapChain == null)
		{
			return;
		}
		
		OnFinishFrame?.Invoke(this);

		if (!m_swapChain.TryPresent(1, 0))
		{
			if (!m_presentError)
			{
				m_presentError = true;
				m_window.RemoveComponent(this);
				m_window.AddComponent(this);
			}
			else
			{
				throw new Exception("Reoccuring DXGI presentation error after device reset.");
			}
		}
		else
		{
			m_presentError = false;
		}
		
	}

	void IWin32WindowComponent.OnMessage(long lTime, nint hWindow, Win32MessageType uMessage, nint wParam, nint lParam)
	{
	}

	private void ResizeSwapchain(int width, int height)
	{
		if (m_context == null || m_swapChain == null)
		{
			return;
		}
		
		m_context.Flush();
		m_context.ClearState();
		if (!m_swapChain.TryResizeBuffers(width, height, DXGIFormat.Unknown))
		{
			throw new Exception("Couldn't resize DXGI swapchain.");
		}
		
	}

	private void CreateSwapchain()
	{
		if (m_context == null || m_device == null || m_factory == null || !m_window.IsAvailable())
		{
			return;
		}

		var description = new DXGISwapChainDescription();
		description.BufferDescription.Width                   = (uint)m_window.GetViewportWidth();
		description.BufferDescription.Height                  = (uint)m_window.GetViewportHeight();
		description.BufferDescription.Format                  = DXGIFormat.R8G8B8A8Unorm;
		description.BufferDescription.RefreshRate.Numerator   = 60;
		description.BufferDescription.RefreshRate.Denominator = 1;
		description.BufferDescription.ScanlineOrdering        = DXGIModeScanlineOrder.Unspecified;
		description.BufferDescription.Scaling                 = DXGIModeScaling.Unspecified;
		description.BufferUsage                               = DXGIUsage.RenderTargetOutput;
		description.BufferCount                               = 3;
		description.OutputWindow                              = m_window.GetHandle();
		description.SampleDescription.Count                   = 1;
		description.Windowed                                  = Win32Bool.True;
		description.SwapEffect                                = DXGISwapEffect.Discard;

		if (!m_factory.CreateSwapChain(m_device.Self, description, out m_swapChain))
		{
			throw new Exception("Couldn't setup DXGI swapchain.");
		}

		if (!m_factory.TryMakeWindowAssociation(m_window.GetHandle(), DXGIMakeWindowAssociationFlags.NoAltEnter))
		{
			throw new Exception("Couldn't configure DXGI window association.");
		}
	}
}
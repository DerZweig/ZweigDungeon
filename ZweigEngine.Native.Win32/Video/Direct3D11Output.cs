using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.DirectX;
using ZweigEngine.Native.Win32.DirectX.Constants;
using ZweigEngine.Native.Win32.DirectX.D3D11;
using ZweigEngine.Native.Win32.DirectX.D3D11.Structures;
using ZweigEngine.Native.Win32.DirectX.DXGI;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Interfaces;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Video;

public class Direct3D11Output : IDisposable, IPlatformVideoOutput, IWin32WindowComponent
{
	private readonly NativeLibraryLoader    m_libraryLoader;
	private readonly Win32Window            m_window;
	private readonly DXGIFactory            m_factory;
	private          D3D11Device?           m_d3d11_device;
	private          D3D11DeviceContext?    m_d3d11_context;
	private          DXGISwapChain?         m_dxgi_swapchain;
	private          D3D11Texture2D?        m_d3d11_swapchain_texture;
	private          D3D11RenderTargetView? m_d3d11_swapchain_target;

	private D3D11Viewport m_viewport;
	private int           m_width;
	private int           m_height;

	public Direct3D11Output(NativeLibraryLoader libraryLoader, Win32Window window)
	{
		m_libraryLoader = libraryLoader;
		m_window        = window;

		if (!DXGIFactory.TryCreate(libraryLoader, out m_factory))
		{
			throw new Exception("Couldn't initialize DXGI factory.");
		}
	}

	private void ReleaseUnmanagedResources()
	{
		m_factory.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Direct3D11Output()
	{
		ReleaseUnmanagedResources();
	}

	public event PlatformVideoDeviceDelegate? OnActivated;
	public event PlatformVideoDeviceDelegate? OnDeactivating;
	public event PlatformVideoDeviceDelegate? OnBeginFrame;
	public event PlatformVideoDeviceDelegate? OnFinishFrame;

	public int GetViewportWidth()  => m_width;
	public int GetViewportHeight() => m_height;

	void IWin32WindowComponent.OnAttach()
	{
		if (m_d3d11_context != null || !m_window.IsAvailable())
		{
			return;
		}

		if (!D3D11Device.TryCreate(m_libraryLoader, null, Direct3DDriverType.Hardware, out m_d3d11_device, out m_d3d11_context))
		{
			throw new Exception("Couldn't initialize D3D11 device.");
		}

		var description = new DXGISwapChainDescription
		{
			BufferDescription = new DXGIModeDescription
			{
				Width            = 0,
				Height           = 0,
				RefreshRate      = default,
				Format           = DXGIFormat.R8G8B8A8Unorm,
				ScanlineOrdering = DXGIModeScanlineOrder.Unspecified,
				Scaling          = DXGIModeScaling.Unspecified
			},
			BufferCount  = 3,
			OutputWindow = 0,
			BufferUsage  = DXGIUsage.RenderTargetOutput,
			SampleDescription = new DXGISampleDescription
			{
				Count   = 1,
				Quality = 0
			},
			Windowed   = Win32Bool.True,
			SwapEffect = DXGISwapEffect.Discard,
			Flags      = 0
		};

		if (!m_factory.CreateSwapChain(m_d3d11_device.Self, description, out m_dxgi_swapchain))
		{
			throw new Exception("Couldn't initialize DXGI swapchain.");
		}

		if (!m_factory.TryMakeWindowAssociation(m_window.GetHandle(), DXGIMakeWindowAssociationFlags.NoAltEnter))
		{
			throw new Exception("Couldn't configure window for DXGI swapchain.");
		}

		CreateSwapchainTarget();

		m_width  = m_window.GetViewportWidth();
		m_height = m_window.GetViewportHeight();
		m_viewport = new D3D11Viewport
		{
			TopLeftX = 0.0f,
			TopLeftY = 0.0f,
			Width    = m_width,
			Height   = m_height,
			MinDepth = 0.0f,
			MaxDepth = 1.0f
		};
		OnActivated?.Invoke(this);
	}

	void IWin32WindowComponent.OnDetach()
	{
		if (m_d3d11_context != null)
		{
			m_d3d11_context.ClearState();
			m_d3d11_context.Flush();
		}

		ReleaseAndReset(ref m_d3d11_swapchain_target);
		ReleaseAndReset(ref m_d3d11_swapchain_texture);
		ReleaseAndReset(ref m_dxgi_swapchain);
		ReleaseAndReset(ref m_d3d11_context);
		ReleaseAndReset(ref m_d3d11_device);
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		if (m_d3d11_context == null || m_dxgi_swapchain == null)
		{
			return;
		}

		var width  = m_window.GetViewportWidth();
		var height = m_window.GetViewportHeight();

		if (width != m_width || height != m_height)
		{
			m_d3d11_context.ClearState();
			m_d3d11_context.Flush();

			ReleaseAndReset(ref m_d3d11_swapchain_target);
			ReleaseAndReset(ref m_d3d11_swapchain_texture);
			if (!m_dxgi_swapchain.TryResizeBuffers(width, height, DXGIFormat.Unknown))
			{
				throw new Exception("Couldn't resize DXGI swapchain.");
			}

			CreateSwapchainTarget();
			m_width  = width;
			m_height = height;
			m_viewport = new D3D11Viewport
			{
				TopLeftX = 0.0f,
				TopLeftY = 0.0f,
				Width    = m_width,
				Height   = m_height,
				MinDepth = 0.0f,
				MaxDepth = 1.0f
			};
		}

		OnBeginFrame?.Invoke(this);
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
		if (m_dxgi_swapchain == null)
		{
			return;
		}

		if (!m_dxgi_swapchain.TryPresent(1, DXGIPresentFlags.None))
		{
			throw new Exception("Unhandled exception while presenting DXGI swapchain.");
		}
	}

	void IWin32WindowComponent.OnMessage(long lTime, IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam)
	{
	}

	private void CreateSwapchainTarget()
	{
		if (!m_dxgi_swapchain!.TryGetBuffer(0, ref m_d3d11_swapchain_texture))
		{
			throw new Exception("Couldn't retrieve DXGI swapchain buffer.");
		}

		if (m_d3d11_device!.CreateRenderTargetView(m_d3d11_swapchain_texture, null, ref m_d3d11_swapchain_target))
		{
			throw new Exception("Couldn't initialize DXGI swapchain render target.");
		}
	}

	private static void ReleaseAndReset<TObject>(ref TObject? disposable) where TObject : DXObject
	{
		disposable?.Dispose();
		disposable = null;
	}
}
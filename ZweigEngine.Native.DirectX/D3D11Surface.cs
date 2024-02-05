using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Native.DirectX.Imports.Constants;
using ZweigEngine.Native.DirectX.Imports.D3D11;
using ZweigEngine.Native.DirectX.Imports.DXGI;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;
using ZweigEngine.Native.DirectX.Imports.DXGI.Structures;
using ZweigEngine.Native.DirectX.Imports.Structures;
using ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

namespace ZweigEngine.Native.DirectX;

public class D3D11Surface : IDisposable, IVideoSurface
{
	private readonly DXGIFactory            m_factory;
	private readonly D3D11Device            m_device;
	private readonly D3D11DeviceContext     m_context;
	private readonly DXGISwapChain          m_swapChain;
	private          D3D11Texture2D?        m_swapChainTexture;
	private          D3D11RenderTargetView? m_swapChainTarget;

	public D3D11Surface(NativeLibraryLoader loader, IPlatformWindow window)
	{
		if (!DXGIFactory.TryCreate(loader, out m_factory))
		{
			throw new Exception("Couldn't initialize DXGI Factory.");
		}

		if (!D3D11Device.TryCreate(loader, null, Direct3DDriverType.Hardware, out m_device, out m_context))
		{
			throw new Exception("Couldn't initialize D3D11 device");
		}

		var swapchainDescription = new DXGISwapChainDescription
		{
			BufferDescription = new DXGIModeDescription
			{
				RefreshRate = new DXGIRational
				{
					Numerator   = 0,
					Denominator = 1
				},
				Format           = DXGIFormat.R8G8B8A8Unorm,
				ScanlineOrdering = DXGIModeScanlineOrder.Unspecified,
				Scaling          = DXGIModeScaling.Unspecified
			},
			SampleDescription = new DXGISampleDescription
			{
				Count   = 1,
				Quality = 0
			},
			BufferUsage  = DXGIUsage.RenderTargetOutput,
			BufferCount  = 3,
			OutputWindow = window.GetNativePointer(),
			Windowed     = Win32Bool.True,
			SwapEffect   = DXGISwapEffect.Discard,
			Flags        = 0
		};

		if (!m_factory.TryMakeWindowAssociation(window.GetNativePointer(), DXGIMakeWindowAssociationFlags.NoAltEnter))
		{
			throw new Exception("Couldn't configure window for DXGI factory.");
		}

		if (!m_factory.CreateSwapChain(m_device.Self, swapchainDescription, out m_swapChain))
		{
			throw new Exception("Couldn't initialize DXGI swap chain.");
		}

		CreateSwapChainTarget();
	}

	private void ReleaseUnmanagedResources()
	{
		m_context.Flush();
		m_context.ClearState();

		m_swapChain.Dispose();
		m_context.Dispose();
		m_device.Dispose();
		m_factory.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~D3D11Surface()
	{
		ReleaseUnmanagedResources();
	}

	public   int                    Width        { get; private set; }
	public   int                    Height       { get; private set; }
	internal D3D11Device            Device       => m_device;
	internal D3D11DeviceContext     Context      => m_context;
	internal D3D11RenderTargetView? RenderTarget => m_swapChainTarget;

	public void Resize(int width, int height)
	{
		m_context.Flush();
		m_context.ClearState();
		m_swapChainTexture?.Dispose();
		m_swapChainTarget?.Dispose();

		if (!m_swapChain.TryResizeBuffers(width, height, DXGIFormat.Unknown))
		{
			throw new Exception("Couldn't resize DXGI swap chain.");
		}

		CreateSwapChainTarget();

		Width  = width;
		Height = height;
	}

	public void Present()
	{
		if (!m_swapChain.TryPresent(1, DXGIPresentFlags.None))
		{
			throw new Exception("Couldn't present DXGI swap chain.");
		}
	}

	private void CreateSwapChainTarget()
	{
		if (!m_swapChain.TryGetBuffer(0, typeof(D3D11Texture2DMethodTable).GUID, ref m_swapChainTexture))
		{
			throw new Exception("Couldn't acquire DXGI swap chain buffer.");
		}

		if (!m_device.CreateRenderTargetView(m_swapChainTexture!, null, ref m_swapChainTarget))
		{
			throw new Exception("Couldn't create render target from DXGI swap chain buffer.");
		}
	}
}
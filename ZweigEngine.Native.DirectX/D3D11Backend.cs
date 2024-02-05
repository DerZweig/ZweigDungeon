using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.DirectX.Imports.D3D11.Structures;

namespace ZweigEngine.Native.DirectX;

public class D3D11Backend : IDisposable, IVideoBackend
{
	private readonly D3D11Surface  m_surface;
	private          D3D11Viewport m_viewport;

	public D3D11Backend(D3D11Surface surface)
	{
		m_surface = surface;
	}

	private void ReleaseUnmanagedResources()
	{
		// TODO release unmanaged resources here
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~D3D11Backend()
	{
		ReleaseUnmanagedResources();
	}

	public void BeginScene(in VideoRect viewport)
	{
		if (m_surface.RenderTarget == null)
		{
			return;
		}

		m_viewport = new D3D11Viewport
		{
			TopLeftX = viewport.Left,
			TopLeftY = viewport.Top,
			Width    = viewport.Width,
			Height   = viewport.Height,
			MinDepth = 0.0f,
			MaxDepth = 1.0f
		};

		m_surface.Context.SetViewport(m_viewport);
		m_surface.Context.SetRenderTarget(m_surface.RenderTarget);
		m_surface.Context.ClearRenderTargetView(m_surface.RenderTarget, 0.0f, 0.0f, 0.0f, 1.0f);
	}

	public void FinishScene()
	{
	}

	public void SetBlendMode(VideoBlendMode mode)
	{
	}

	public void FlushPending()
	{
	}

	public void CreateImage(ushort width, ushort height, IntPtr data, out uint name)
	{
		name = 0u;
	}

	public void DestroyImage(uint name)
	{
	}

	public void BindImage(uint? name)
	{
	}

	public void UploadImage(uint name, ushort width, ushort height, IntPtr data)
	{
	}

	public void DrawImage(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags)
	{
	}
}
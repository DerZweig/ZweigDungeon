using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigEngine.Native.Win32;

public sealed class D3D11VideoContext : IDisposable, IVideoContext
{
	private readonly D3D11VideoDevice m_video;

	public D3D11VideoContext(D3D11VideoDevice video)
	{
		m_video                =  video;
		m_video.OnActivated    += HandleVideoActivated;
		m_video.OnDeactivating += HandleVideoDeactivating;
		m_video.OnBeginFrame   += HandleVideoBeginFrame;
		m_video.OnFinishFrame  += HandleVideoFinishFrame;
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

	~D3D11VideoContext()
	{
		ReleaseUnmanagedResources();
	}

	private void HandleVideoActivated(IPlatformVideo video)
	{
	}

	private void HandleVideoDeactivating(IPlatformVideo video)
	{
	}

	private void HandleVideoBeginFrame(IPlatformVideo video)
	{
	}

	private void HandleVideoFinishFrame(IPlatformVideo video)
	{
	}

	public void SetBlendMode(VideoBlendMode mode)
	{
	}

	public void CreateSurface(ushort width, ushort height, out IVideoImage image)
	{
		image = null!;
	}
}
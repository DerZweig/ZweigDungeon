using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Services.Video;

public class VideoContext
{
	private readonly IPlatformVideo               m_video;
	private readonly IPlatformWindow              m_window;
	private readonly Dictionary<VideoImage, uint> m_mappings;
	private          VideoImage?                  m_active;
	private          IVideoBackend?               m_backend;

	public VideoContext(IPlatformVideo video, IPlatformWindow window)
	{
		m_video    = video;
		m_window   = window;
		m_mappings = new Dictionary<VideoImage, uint>();

		m_video.OnActivated    += HandleVideoActivated;
		m_video.OnDeactivating += HandleVideoDeactivating;
		m_video.OnBeginFrame   += HandleVideoBeginFrame;
		m_video.OnFinishFrame  += HandleVideoFinishFrame;
	}

	private void HandleVideoActivated(IPlatformVideo video, IVideoBackend backend)
	{
		m_backend = backend;
	}

	private void HandleVideoDeactivating(IPlatformVideo video)
	{
		m_mappings.Clear();
		m_active = null;
	}

	private void HandleVideoBeginFrame(IPlatformVideo video)
	{
		if (m_backend != null)
		{
			m_backend.BeginScene(new VideoRect
			{
				Left   = 0,
				Top    = 0,
				Width  = m_window.GetViewportWidth(),
				Height = m_window.GetViewportHeight()
			});
			m_backend.SetBlendMode(VideoBlendMode.Default);
		}
	}

	private void HandleVideoFinishFrame(IPlatformVideo video)
	{
		m_backend?.FinishScene();
	}

	public void SetBlendMode(VideoBlendMode mode)
	{
		if (m_backend != null)
		{
			m_backend.FlushPending();
			m_backend.SetBlendMode(mode);
		}
	}

	public void CreateSurface(ushort width, ushort height, out IVideoImage image)
	{
		image = new VideoImage(this, width, height);
	}

	internal void DestroyImage(VideoImage image)
	{
		if (m_mappings.Remove(image, out var name))
		{
			BindImage(null);
			m_backend?.DestroyImage(name);
		}
	}

	internal void Map(VideoImage image, Action<VideoColor[]> mapper)
	{
		BindImage(null);
		mapper(image.Data);
		if (m_backend != null)
		{
			if (!m_mappings.TryGetValue(image, out var name))
			{
				m_backend.CreateImage(image.Width, image.Height, image.Address, out name);
				m_mappings[image] = name;
			}
			else
			{
				m_backend.UploadImage(name, image.Width, image.Height, image.Address);
			}

			m_backend.BindImage(name);
			m_active = image;
		}
	}

	internal void DrawImage(VideoImage image, in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags)
	{
		BindImage(image);
		m_backend?.DrawImage(dstRegion, srcRegion, tintColor, blitFlags);
	}

	private void BindImage(VideoImage? image)
	{
		if (m_active == image || m_backend == null)
		{
			return;
		}

		m_backend.FlushPending();
		if (image != null)
		{
			if (!m_mappings.TryGetValue(image, out var name))
			{
				m_backend.BindImage(null);
				m_backend.CreateImage(image.Width, image.Height, image.Address, out name);
				m_mappings[image] = name;
			}

			m_backend.BindImage(name);
		}
		else
		{
			m_backend.BindImage(null);
		}

		m_active = image;
	}
}
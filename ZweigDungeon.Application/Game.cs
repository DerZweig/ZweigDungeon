using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class Game : IDisposable, IWindowListener
{
	private readonly IVideoContext           m_video;
	private readonly IDisposable             m_subscription;
	private readonly CancellationTokenSource m_cancellationTokenSource;
	private          IVideoSurface?          m_testSurface;

	public Game(MessageBus messageBus, IVideoContext video)
	{
		m_video                   = video;
		m_subscription            = messageBus.Subscribe<IWindowListener>(this);
		m_cancellationTokenSource = new CancellationTokenSource();
	}

	private void ReleaseUnmanagedResources()
	{
		m_cancellationTokenSource.Dispose();
	}

	private void Dispose(bool disposing)
	{
		ReleaseUnmanagedResources();
		if (disposing)
		{
			m_subscription.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~Game()
	{
		Dispose(false);
	}

	public void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetMinimumSize(640, 480);
		window.Show();
		m_video.CreateSurface(2, 2, out m_testSurface);
		m_video.MapSurfaceData(m_testSurface, pixels =>
		{
			pixels[0] = new VideoColor { Red = 255, Green = 0, Blue = 0, Alpha = 255 };
			pixels[1] = new VideoColor { Red = 0, Green = 255, Blue = 0, Alpha = 255 };
			pixels[2] = new VideoColor { Red = 0, Green = 0, Blue = 255, Alpha = 255 };
			pixels[3] = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
		});
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_testSurface?.Dispose();
		m_cancellationTokenSource.Cancel();
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width  = window.GetViewportWidth();
		var height = window.GetViewportHeight();
		m_video.BeginFrame(width, height);

		if (m_testSurface != null)
		{
			var dst = new VideoRect { Left = 0, Top     = 0, Width  = 64, Height = 64 };
			var src = new VideoRect { Left = 0, Top     = 0, Width  = 2, Height = 2 };
			var col = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha   = 255 };

			m_video.DrawSurface(m_testSurface, dst, src, col, VideoFlags.None);
			dst.Left += 72;
			m_video.DrawSurface(m_testSurface, dst, src, col, VideoFlags.MirrorHorizontal);
			dst.Left += 72;
			m_video.DrawSurface(m_testSurface, dst, src, col, VideoFlags.MirrorVertical);
			dst.Left += 72;
			m_video.DrawSurface(m_testSurface, dst, src, col, VideoFlags.MirrorHorizontal | VideoFlags.MirrorVertical);
		}

		m_video.FinishFrame();
	}
}
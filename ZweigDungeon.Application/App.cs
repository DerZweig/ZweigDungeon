using ZweigDungeon.Application.Manager.Constants;
using ZweigDungeon.Application.Manager.Implementation;
using ZweigDungeon.Application.Manager.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Platform.Messages;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class App : IDisposable, IWindowListener
{
	private readonly IVideoContext           m_video;
	private readonly IFontManager            m_fontManager;
	private readonly IImageManager           m_imageManager;
	private readonly IDisposable             m_subscription;
	private readonly CancellationTokenSource m_cancellation;

	public App(MessageBus messageBus, IVideoContext video, IFontManager fontManager, IImageManager imageManager)
	{
		m_video        = video;
		m_fontManager  = fontManager;
		m_imageManager = imageManager;
		m_subscription = messageBus.Subscribe<IWindowListener>(this);
		m_cancellation = new CancellationTokenSource();
	}

	private void ReleaseUnmanagedResources()
	{
		m_subscription.Dispose();
		m_cancellation.Dispose();
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

	public void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetStyle(true, true);
		window.SetMinimumSize(640, 480);
		window.Show();
		m_imageManager.Load("Garbage");
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_cancellation.Cancel();
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width      = window.GetViewportWidth();
		var height     = window.GetViewportHeight();
		var textLayout = width / 3;

		var text = @"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. 

At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

		m_fontManager.Layout(FontSize.Small, textLayout, text, out var small);
		m_fontManager.Draw(FontSize.Small, small, 100, 50,
		                   new VideoRect { Left = 100, Top   = 50, Width = textLayout, Height = 200 },
		                   new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha         = 255 });

		m_fontManager.Layout(FontSize.Medium, textLayout, text, out var larger);
		m_fontManager.Draw(FontSize.Medium, larger, 100, 300,
		                   new VideoRect { Left = 100, Top   = 300, Width = textLayout, Height = 200 },
		                   new VideoColor { Red = 255, Green = 255, Blue  = 255, Alpha         = 255 });
	}
}
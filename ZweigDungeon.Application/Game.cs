using System.Diagnostics;
using ZweigDungeon.Common.Constants;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class Game : IDisposable, IWindowListener, IKeyboardListener
{
	private readonly IVideoContext           m_video;
	private readonly IDisposable             m_subscription;
	private readonly IDisposable             m_keyboard;
	private readonly CancellationTokenSource m_cancellationTokenSource;

	public Game(MessageBus messageBus, IVideoContext video)
	{
		m_video                   = video;
		m_subscription            = messageBus.Subscribe<IWindowListener>(this);
		m_keyboard                = messageBus.Subscribe<IKeyboardListener>(this);
		m_cancellationTokenSource = new CancellationTokenSource();
	}

	private void ReleaseUnmanagedResources()
	{
		m_cancellationTokenSource.Dispose();
		m_subscription.Dispose();
		m_keyboard.Dispose();
	}


	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Game()
	{
		ReleaseUnmanagedResources();
	}

	public void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetMinimumSize(640, 480);
		window.Show();
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_cancellationTokenSource.Cancel();
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width  = window.GetViewportWidth();
		var height = window.GetViewportHeight();
		m_video.BeginFrame(width, height);

		m_video.FinishFrame();
	}

	public void KeyPressed(IPlatformKeyboard keyboard, KeyboardKey key)
	{
		if (Debugger.IsAttached)
		{
			Debug.WriteLine($"{nameof(KeyPressed)} {key}");
		}
	}

	public void KeyReleased(IPlatformKeyboard keyboard, KeyboardKey key)
	{
		if (Debugger.IsAttached)
		{
			Debug.WriteLine($"{nameof(KeyReleased)} {key}");
		}
	}

	public void KeyTyped(IPlatformKeyboard keyboard, char character)
	{
	}
}
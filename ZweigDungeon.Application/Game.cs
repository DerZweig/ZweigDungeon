﻿using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class Game : IDisposable, IWindowListener
{
	private readonly IVideoContext           m_video;
	private readonly IDisposable             m_subscription;
	private readonly CancellationTokenSource m_cancellationTokenSource;

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
		
		//do drawing
		
		m_video.FinishFrame();
	}
}
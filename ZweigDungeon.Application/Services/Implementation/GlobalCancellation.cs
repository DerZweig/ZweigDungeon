using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;

namespace ZweigDungeon.Application.Services.Implementation;

public class GlobalCancellation : IDisposable, IGlobalCancellation
{
	private readonly IPlatformWindow         m_window;
	private readonly CancellationTokenSource m_cancellation;

	public GlobalCancellation(IPlatformWindow window)
	{
		m_window         =  window;
		m_cancellation   =  new CancellationTokenSource();
		window.OnClosing += HandleWindowClosing;
	}

	private void ReleaseUnmanagedResources()
	{
		m_cancellation.Cancel();
		m_window.OnClosing -= HandleWindowClosing;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~GlobalCancellation()
	{
		ReleaseUnmanagedResources();
	}

	public CancellationToken Token => m_cancellation.Token;

	private void HandleWindowClosing(IPlatformWindow window)
	{
		m_cancellation.Cancel();
	}
}
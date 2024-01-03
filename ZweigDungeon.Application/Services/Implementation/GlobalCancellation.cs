using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Platform.Messages;

namespace ZweigDungeon.Application.Services.Implementation;

public class GlobalCancellation : IDisposable, IWindowListener, IGlobalCancellation
{
	private readonly CancellationTokenSource m_cancellation;
	
	public GlobalCancellation()
	{
		m_cancellation = new CancellationTokenSource();
	}
	
	private void ReleaseUnmanagedResources()
	{
		m_cancellation.Cancel();
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

	public void WindowCreated(IPlatformWindow window)
	{
		
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_cancellation.Cancel();
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
	}

	public CancellationToken Token => m_cancellation.Token;
}
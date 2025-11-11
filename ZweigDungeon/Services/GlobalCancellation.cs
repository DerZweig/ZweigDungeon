using ZweigEngine.Common.Services;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services;

internal sealed class GlobalCancellation : DisposableObject, IGlobalCancellation
{
    private readonly CancellationTokenSource m_cancellation;

    public GlobalCancellation()
    {
        m_cancellation = new CancellationTokenSource();
        Token          = m_cancellation.Token;
    }

    protected override void ReleaseUnmanagedResources()
    {
        m_cancellation.Dispose();
    }

    public CancellationToken Token { get; }

    public void Cancel()
    {
        if (!m_cancellation.IsCancellationRequested)
        {
            m_cancellation.Cancel();
        }
    }
}
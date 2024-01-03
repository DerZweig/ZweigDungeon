using ZweigEngine.Common.Utility.Concurrency;

namespace ZweigEngine.Common.Services.Messages;

internal class MessageSubscription<TInterface> : IDisposable where TInterface : class
{
	private readonly TInterface        m_value;
	private          ConcurrentBoolean m_cancelled;

	public MessageSubscription(in TInterface value)
	{
		m_value     = value;
		m_cancelled = new ConcurrentBoolean(false);
	}

	public bool IsCancelled()
	{
		return m_cancelled.Read();
	}

	public bool TryInvoke(in Action<TInterface> work)
	{
		if (m_cancelled.Read())
		{
			return false;
		}

		work(m_value);
		return true;

	}

	private void ReleaseUnmanagedResources()
	{
		m_cancelled.Exchange(true);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~MessageSubscription()
	{
		ReleaseUnmanagedResources();
	}
}
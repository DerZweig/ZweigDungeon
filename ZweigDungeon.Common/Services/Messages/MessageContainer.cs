namespace ZweigDungeon.Common.Services.Messages;

internal class MessageContainer<TInterface> where TInterface : class
{
	private readonly List<MessageSubscription<TInterface>> m_subscriptions;
	private readonly ReaderWriterLockSlim                  m_synchronize;

	public MessageContainer()
	{
		m_subscriptions = new List<MessageSubscription<TInterface>>();
		m_synchronize   = new ReaderWriterLockSlim();
	}

	public void Invoke(in Action<TInterface> work)
	{
		MessageSubscription<TInterface>[] copy;
		m_synchronize.EnterReadLock();
		try
		{
			copy = m_subscriptions.ToArray();
		}
		finally
		{
			m_synchronize.ExitReadLock();
		}

		var removed = false;
		foreach (var subscription in copy)
		{
			if (!subscription.TryInvoke(work))
			{
				removed = true;
			}
		}

		if (removed)
		{
			m_synchronize.EnterWriteLock();
			try
			{
				m_subscriptions.RemoveAll(x => x.IsCancelled());
			}
			finally
			{
				m_synchronize.ExitWriteLock();
			}
		}
	}

	public IDisposable Insert(in TInterface handler)
	{
		var subscription = new MessageSubscription<TInterface>(handler);

		m_synchronize.EnterWriteLock();
		try
		{
			m_subscriptions.Add(subscription);
			return subscription;
		}
		finally
		{
			m_synchronize.ExitWriteLock();
		}
	}
}
namespace ZweigEngine.Common.Utility.Concurrency;

public class ExclusiveTaskFactory
{
	private readonly TaskFactory m_factory;

	public ExclusiveTaskFactory()
	{
		m_factory = new TaskFactory(CancellationToken.None,
		                            TaskCreationOptions.DenyChildAttach | TaskCreationOptions.PreferFairness,
		                            TaskContinuationOptions.None,
		                            new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
	}

	public Task Invoke(Action work)
	{
		return m_factory.StartNew(work);
	}

	public Task Invoke(Action work, CancellationToken cancellationToken)
	{
		return m_factory.StartNew(work, cancellationToken);
	}

	public Task<TResult> Invoke<TResult>(Func<TResult> work)
	{
		return m_factory.StartNew(work);
	}

	public Task<TResult> Invoke<TResult>(Func<TResult> work, CancellationToken cancellationToken)
	{
		return m_factory.StartNew(work, cancellationToken);
	}
	
	public Task Invoke(Func<Task> work)
	{
		return m_factory.StartNew(work).Unwrap();
	}

	public Task Invoke(Func<Task> work, CancellationToken cancellationToken)
	{
		return m_factory.StartNew(work, cancellationToken).Unwrap();
	}

	public Task<TResult> Invoke<TResult>(Func<Task<TResult>> work)
	{
		return m_factory.StartNew(work).Unwrap();
	}

	public Task<TResult> Invoke<TResult>(Func<Task<TResult>> work, CancellationToken cancellationToken)
	{
		return m_factory.StartNew(work, cancellationToken).Unwrap();
	}
}
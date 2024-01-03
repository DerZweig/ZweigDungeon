using System.Collections.Concurrent;
using ZweigEngine.Common.Interfaces.Platform;

namespace ZweigEngine.Native.Win32;

public sealed class Win32SynchronizationContext : SynchronizationContext, IPlatformSynchronization
{
	private readonly ConcurrentQueue<Action> m_pending;
	private readonly TaskFactory             m_factory;

	public Win32SynchronizationContext()
	{
		var previous = Current;
		try
		{
			SetSynchronizationContext(this);
			m_pending = new ConcurrentQueue<Action>();
			m_factory = new TaskFactory(CancellationToken.None,
			                            TaskCreationOptions.DenyChildAttach,
			                            TaskContinuationOptions.None,
			                            TaskScheduler.FromCurrentSynchronizationContext());
		}
		finally
		{
			SetSynchronizationContext(previous);
		}
	}

	public override void Post(SendOrPostCallback d, object? state)
	{
		if (Current == this)
		{
			d(state);
		}
		else
		{
			m_pending.Enqueue(() => d(state));
		}
	}

	public override void Send(SendOrPostCallback d, object? state)
	{
		throw new NotSupportedException();
	}

	public override SynchronizationContext CreateCopy()
	{
		return this;
	}

	internal void ExecuteWithoutPending(Action action)
	{
		if (Current != this)
		{
			var previous = Current;

			try
			{
				SetSynchronizationContext(this);
				action();
			}
			finally
			{
				SetSynchronizationContext(previous);
			}
		}
		else
		{
			action();
		}
	}

	internal void Execute(Action action)
	{
		if (Current != this)
		{
			var previous = Current;

			try
			{
				SetSynchronizationContext(this);
				ExecutePending();
				action();
			}
			finally
			{
				SetSynchronizationContext(previous);
			}
		}
		else
		{
			ExecutePending();
			action();
		}
	}

	private void ExecutePending()
	{
		while (m_pending.TryDequeue(out var work))
		{
			work();
		}
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
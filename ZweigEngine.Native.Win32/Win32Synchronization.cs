using System.Collections.Concurrent;
using ZweigEngine.Common.Interfaces.Platform;

namespace ZweigEngine.Native.Win32;

public class Win32Synchronization : IPlatformSynchronization
{
	private readonly ConcurrentQueue<Action> m_queue;
	private readonly Context                 m_context;
	private readonly TaskFactory             m_factory;

	public Win32Synchronization()
	{
		m_queue   = new ConcurrentQueue<Action>();
		m_context = new Context(m_queue);
		m_factory = new TaskFactory(CancellationToken.None,
		                            TaskCreationOptions.DenyChildAttach,
		                            TaskContinuationOptions.None,
		                            new Scheduler(m_queue));
	}

	public void ExecuteWithoutPending(Action action)
	{
		if (SynchronizationContext.Current != m_context)
		{
			var previous = SynchronizationContext.Current;

			try
			{
				SynchronizationContext.SetSynchronizationContext(m_context);
				action();
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(previous);
			}
		}
		else
		{
			action();
		}
	}

	public void Execute(Action action)
	{
		if (SynchronizationContext.Current != m_context)
		{
			var previous = SynchronizationContext.Current;

			try
			{
				SynchronizationContext.SetSynchronizationContext(m_context);
				ExecutePending();
				action();
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(previous);
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
		while (m_queue.TryDequeue(out var work))
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

	private class Context : SynchronizationContext
	{
		private readonly ConcurrentQueue<Action> m_queue;

		public Context(ConcurrentQueue<Action> queue)
		{
			m_queue = queue;
		}

		public override void Post(SendOrPostCallback d, object? state)
		{
			m_queue.Enqueue(() => d(state));
		}

		public override void Send(SendOrPostCallback d, object? state)
		{
			throw new NotSupportedException();
		}

		public override SynchronizationContext CreateCopy()
		{
			throw new NotSupportedException();
		}
	}

	private class Scheduler : TaskScheduler
	{
		private readonly ConcurrentQueue<Action> m_queue;

		public Scheduler(ConcurrentQueue<Action> queue)
		{
			m_queue = queue;
		}

		protected override IEnumerable<Task>? GetScheduledTasks()
		{
			return null;
		}

		protected override void QueueTask(Task task)
		{
			m_queue.Enqueue(() => TryExecuteTask(task));
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return false;
		}
	}
}
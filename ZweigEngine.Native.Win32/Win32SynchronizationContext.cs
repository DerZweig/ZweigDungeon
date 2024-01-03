using System.Collections.Concurrent;

namespace ZweigEngine.Native.Win32;

internal sealed class Win32SynchronizationContext : SynchronizationContext
{
	private readonly ConcurrentQueue<Action> m_pending;

	public Win32SynchronizationContext()
	{
		m_pending = new ConcurrentQueue<Action>();
	}

	public override void Post(SendOrPostCallback d, object? state)
	{
		m_pending.Enqueue(() => d(state));
	}

	public override void Send(SendOrPostCallback d, object? state)
	{
		throw new NotSupportedException();
	}

	public override SynchronizationContext CreateCopy()
	{
		throw new NotSupportedException();
	}

	public void ExecuteWithoutPending(Action action)
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

	public void Execute(Action action)
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
}
namespace ZweigEngine.Common.Interfaces.Platform;

public interface IPlatformSynchronization
{
	void          ExecuteWithGuard(Action work);
	TResult       ExecuteWithGuard<TResult>(Func<TResult> work);
	Task          Invoke(Action work);
	Task          Invoke(Action work, CancellationToken cancellationToken);
	Task<TResult> Invoke<TResult>(Func<TResult> work);
	Task<TResult> Invoke<TResult>(Func<TResult> work, CancellationToken cancellationToken);
	Task          Invoke(Func<Task> work);
	Task          Invoke(Func<Task> work, CancellationToken cancellationToken);
	Task<TResult> Invoke<TResult>(Func<Task<TResult>> work);
	Task<TResult> Invoke<TResult>(Func<Task<TResult>> work, CancellationToken cancellationToken);
}
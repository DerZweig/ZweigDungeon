namespace ZweigDungeon.Common.Services;

public sealed class ServiceProvider : IServiceProvider, IDisposable
{
	private readonly Dictionary<Type, object?> m_singletons;
	private readonly Stack<IDisposable>        m_disposables;

	internal ServiceProvider()
	{
		m_singletons  = new Dictionary<Type, object?>();
		m_disposables = new Stack<IDisposable>();
	}

	internal void AddSingleton(Type type, object instance)
	{
		m_singletons.Add(type, instance);
	}

	internal void AddDestructor(IDisposable disposable)
	{
		m_disposables.Push(disposable);
	}

	public object? GetService(Type serviceType)
	{
		if (serviceType == typeof(IServiceProvider) || serviceType == typeof(ServiceProvider))
		{
			return this;
		}

		if (m_singletons.TryGetValue(serviceType, out var service))
		{
			return service;
		}

		return null;
	}

	private void ReleaseUnmanagedResources()
	{
		while (m_disposables.TryPop(out var disposable))
		{
			try
			{
				disposable.Dispose();
			}
			catch
			{
				//ignored
			}
		}
		
		m_disposables.Clear();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~ServiceProvider()
	{
		ReleaseUnmanagedResources();
	}
}
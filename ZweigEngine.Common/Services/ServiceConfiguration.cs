using System.Reflection;

namespace ZweigEngine.Common.Services;

public sealed class ServiceConfiguration
{
	private readonly List<Entry> m_entries;

	public ServiceConfiguration()
	{
		m_entries = new List<Entry>();
	}

	public void AddSingleton(Type interfaceType, Type implementationType)
	{
		m_entries.Add(new Entry
		{
			InterfaceType      = interfaceType,
			ImplementationType = implementationType
		});
	}

	public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface
	{
		m_entries.Add(new Entry
		{
			InterfaceType      = typeof(TInterface),
			ImplementationType = typeof(TImplementation)
		});
	}

	public void AddSingleton(Type implementationType)
	{
		m_entries.Add(new Entry
		{
			InterfaceType      = implementationType,
			ImplementationType = implementationType
		});
	}

	public void AddSingleton<TImplementation>()
	{
		m_entries.Add(new Entry
		{
			InterfaceType      = typeof(TImplementation),
			ImplementationType = typeof(TImplementation)
		});
	}

	public ServiceProvider Build()
	{
		var serviceProvider = new ServiceProvider();
		var pending         = new Queue<ServiceDescriptor>();

		var interfaces = new HashSet<Type>
		{
			typeof(IServiceProvider)
		};

		foreach (var entry in m_entries)
		{
			if (!interfaces.Add(entry.InterfaceType))
			{
				throw new InvalidOperationException($"Duplicate service registration of type {entry.InterfaceType}");
			}

			if (entry.InterfaceType != entry.ImplementationType)
			{
				interfaces.Add(entry.ImplementationType);
			}
		}

		foreach (var entry in m_entries)
		{
			var type = entry.ImplementationType;
			var constructors = type.GetConstructors().Select(ctor => new ServiceDescriptor
			{
				InterfaceType      = entry.InterfaceType,
				ImplementationType = entry.ImplementationType,
				Constructor        = ctor,
				ParameterTypes     = ctor.GetParameters().Select(param => param.ParameterType).ToArray()
			}).ToArray();

			var sorted = constructors.OrderByDescending(x => x.ParameterTypes.Length);
			var best   = sorted.FirstOrDefault();

			if (best == null || best.ParameterTypes.Any(p => !interfaces.Contains(p)))
			{
				throw new InvalidOperationException($"Couldn't find suitable constructor for service {type.Name}");
			}

			pending.Enqueue(best);
		}

		while (pending.Any())
		{
			var pendingCount      = pending.Count;
			var unresolvedPending = new Queue<ServiceDescriptor>();

			while (pending.TryDequeue(out var descriptor))
			{
				var parameters = descriptor.ParameterTypes.Select(x => serviceProvider.GetService(x)).ToArray();
				if (parameters.Any(x => x == null))
				{
					unresolvedPending.Enqueue(descriptor);
				}
				else
				{
					var instance = descriptor.Constructor.Invoke(parameters);
					serviceProvider.AddSingleton(descriptor.InterfaceType, instance);

					if (descriptor.InterfaceType != descriptor.ImplementationType)
					{
						serviceProvider.AddSingleton(descriptor.ImplementationType, instance);
					}

					if (instance is IDisposable disposable)
					{
						serviceProvider.AddDestructor(disposable);
					}
				}
			}

			if (unresolvedPending.Count == pendingCount)
			{
				throw new InvalidOperationException("Couldn't resolve one or more service dependencies. Please check for circular constructor paths.");
			}

			while (unresolvedPending.TryDequeue(out var item))
			{
				pending.Enqueue(item);
			}
		}

		return serviceProvider;
	}

	private class Entry
	{
		public Type InterfaceType      { get; init; } = null!;
		public Type ImplementationType { get; init; } = null!;
	}

	private class ServiceDescriptor
	{
		public Type            InterfaceType      { get; init; } = null!;
		public Type            ImplementationType { get; init; } = null!;
		public ConstructorInfo Constructor        { get; init; } = null!;
		public Type[]          ParameterTypes     { get; init; } = null!;
	}
}
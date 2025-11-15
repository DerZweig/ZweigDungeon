using System.Reflection;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Common.Services.ServiceProvider;

public class ScopedServiceProvider : DisposableObject, IServiceProvider
{
    private delegate bool InitializerDelegate(ScopedServiceProvider provider);

    private readonly Dictionary<Type, object> m_instances;
    private readonly Stack<IDisposable>       m_disposables;

    private ScopedServiceProvider()
    {
        m_instances   = new Dictionary<Type, object>();
        m_disposables = new Stack<IDisposable>();
        m_instances.Add(typeof(IServiceProvider), new ServiceProviderReadonly(m_instances));
    }

    protected override void ReleaseUnmanagedResources()
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

        m_instances.Clear();
    }

    public object? GetService(Type serviceType)
    {
        return m_instances.GetValueOrDefault(serviceType);
    }

    private bool IsKnownType(Type serviceType)
    {
        return m_instances.ContainsKey(serviceType);
    }

    public static ScopedServiceProvider Create(ScopedServiceProvider parent, Action<IServiceConfiguration> configuration)
    {
        return CreateInternal(parent, configuration);
    }

    public static ScopedServiceProvider Create(Action<IServiceConfiguration> configuration)
    {
        return CreateInternal(null, configuration);
    }

    private static ScopedServiceProvider CreateInternal(ScopedServiceProvider? parent, Action<IServiceConfiguration> configuration)
    {
        var result = new ScopedServiceProvider();
        if (parent != null)
        {
            foreach (var kv in parent.m_instances.Where(kv => !result.IsKnownType(kv.Key)))
            {
                result.m_instances[kv.Key] = kv.Value;
            }
        }

        try
        {
            var config = new ServiceConfiguration();

            configuration(config);

            var pending    = CreateInitializerQueue(config);
            var unresolved = new Queue<InitializerDelegate>();

            while (pending.Count != 0)
            {
                var beforeCount = pending.Count;
                while (pending.TryDequeue(out var work))
                {
                    if (!work(result))
                    {
                        unresolved.Enqueue(work);
                    }
                }

                if (beforeCount == unresolved.Count)
                {
                    throw new Exception("Stopped processing service initializers, check for circular dependencies.");
                }

                while (unresolved.TryDequeue(out var work))
                {
                    pending.Enqueue(work);
                }
            }
        }
        catch
        {
            result.Dispose();
            throw;
        }

        return result;
    }

    private static Queue<InitializerDelegate> CreateInitializerQueue(ServiceConfiguration config)
    {
        var result       = new Queue<InitializerDelegate>();
        var types        = new HashSet<Type>();
        var constructors = new Dictionary<Type, ConstructorInfo>();

        foreach (var step in config.EnumerateSteps())
        {
            if (step.factory != null)
            {
                types.Add(step.type);
                result.Enqueue(host =>
                {
                    var instance = step.factory()!;
                    if (instance is IDisposable disposable)
                    {
                        host.m_disposables.Push(disposable);
                    }

                    host.m_instances.Add(step.type, instance);
                    return true;
                });
            }
            else if (types.Add(step.type))
            {
                result.Enqueue(host =>
                {
                    if (!constructors.TryGetValue(step.type, out var constructor))
                    {
                        constructor = GenerateConstructor(host, step.type);
                        constructors.Add(step.type, constructor);
                    }

                    var init = CreateInstanceInitializer(step.type, constructor);
                    if (!init(host))
                    {
                        result.Enqueue(init);
                    }

                    return true;
                });
            }

            if (step.alias != null)
            {
                result.Enqueue(CreateAliasInitializer(step.type, step.alias));
            }
        }

        return result;
    }

    private static InitializerDelegate CreateAliasInitializer(Type type, Type alias) => host =>
    {
        var typeInstance = host.GetService(type);
        if (typeInstance == null)
        {
            return false;
        }

        var aliasInstance = host.GetService(alias);
        if (aliasInstance != null)
        {
            return ReferenceEquals(aliasInstance, typeInstance);
        }

        host.m_instances.Add(alias, typeInstance);
        return true;
    };

    private static InitializerDelegate CreateInstanceInitializer(Type type, ConstructorInfo constructor) => host =>
    {
        var parameters = constructor.GetParameters().Select(x => x.ParameterType).Select(host.GetService).ToArray();
        if (parameters.Any(x => x == null))
        {
            return false;
        }

        var instance = constructor.Invoke(parameters);
        if (instance is IDisposable disposable)
        {
            host.m_disposables.Push(disposable);
        }

        host.m_instances.Add(type, instance);
        return true;
    };

    private static ConstructorInfo GenerateConstructor(ScopedServiceProvider host, Type type)
    {
        var constructor        = (ConstructorInfo?)null;
        var selectedParameters = Array.Empty<Type>();
        var typeConstructors   = type.GetConstructors();

        foreach (var typeConstructor in typeConstructors)
        {
            var typeParameters = typeConstructor.GetParameters();
            if (typeParameters.Length <= selectedParameters.Length && constructor != null)
            {
                continue;
            }

            var parameterTypes = typeParameters.Select(x => x.ParameterType).ToArray();
            if (parameterTypes.Any(x => !host.IsKnownType(x)))
            {
                continue;
            }

            constructor        = typeConstructor;
            selectedParameters = parameterTypes;
        }

        if (constructor == null)
        {
            throw new Exception($"Couldn't find suitable constructor for type {type.Name}.");
        }

        return constructor;
    }
}
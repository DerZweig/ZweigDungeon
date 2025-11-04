namespace ZweigEngine.Common.Services;

internal class ServiceProviderReadonly : IServiceProvider
{
    private readonly IReadOnlyDictionary<Type, object> m_instances;

    public ServiceProviderReadonly(IReadOnlyDictionary<Type, object> instances)
    {
        m_instances = instances;
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(IServiceProvider))
        {
            return this;
        }

        return m_instances.GetValueOrDefault(serviceType);
    }
}
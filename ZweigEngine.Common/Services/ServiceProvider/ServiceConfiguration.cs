namespace ZweigEngine.Common.Services.ServiceProvider;

internal class ServiceConfiguration : IServiceConfiguration
{
    private readonly List<Step> m_steps;

    public ServiceConfiguration()
    {
        m_steps = new List<Step>();
    }

    public IEnumerable<Step> EnumerateSteps()
    {
        return m_steps;
    }

    public void AddFactory<TImplementation>(Func<TImplementation> value)
    {
        m_steps.Add(new Step
        {
            type    = typeof(TImplementation),
            alias   = null,
            factory = () => value()!
        });
    }

    public void AddSingleton<TImplementation>()
    {
        m_steps.Add(new Step
        {
            type    = typeof(TImplementation),
            alias   = null,
            factory = null
        });
    }

    public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface
    {
        m_steps.Add(new Step
        {
            type    = typeof(TImplementation),
            alias   = typeof(TInterface),
            factory = null
        });
    }

    public struct Step
    {
        public Type          type;
        public Type?         alias;
        public Func<object>? factory;
    }
}
namespace ZweigEngine.Common.Services;

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
    
    public void AddSingleton<TImplementation>()
    {
        m_steps.Add(new Step
        {
            type  = typeof(TImplementation),
            alias = null
        });
    }

    public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface
    {
        m_steps.Add(new Step
        {
            type  = typeof(TImplementation),
            alias = typeof(TInterface)
        });
    }

    public struct Step
    {
        public Type  type;
        public Type? alias;
    }
}
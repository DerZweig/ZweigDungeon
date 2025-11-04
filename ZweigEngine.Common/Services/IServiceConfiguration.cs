namespace ZweigEngine.Common.Services;

public interface IServiceConfiguration
{
    void AddSingleton<TImplementation>();
    void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
    
}
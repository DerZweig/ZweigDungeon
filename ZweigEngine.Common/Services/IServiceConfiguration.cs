namespace ZweigEngine.Common.Services;

public interface IServiceConfiguration
{
    void AddFactory<TImplementation>(Func<TImplementation> value);
    void AddSingleton<TImplementation>();
    void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
}
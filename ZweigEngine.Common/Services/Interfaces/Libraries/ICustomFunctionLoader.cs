namespace ZweigEngine.Common.Services.Interfaces.Libraries;

public interface ICustomFunctionLoader
{
	void LoadFunction<TDelegate>(string exportName, out TDelegate func) where TDelegate : Delegate;
	bool TryLoadFunction<TDelegate>(string exportName, out TDelegate? func) where TDelegate : Delegate;
}
namespace ZweigEngine.Common.Services;

public interface IApplicationMetaData
{
    string                Title            { get; }
    string                Version          { get; }
    IReadOnlyList<string> StartupArguments { get; }
}
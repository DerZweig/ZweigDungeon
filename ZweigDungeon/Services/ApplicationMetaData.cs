using ZweigEngine.Common.Services;

namespace ZweigDungeon.Services;

internal sealed class ApplicationMetaData : IApplicationMetaData
{
    public ApplicationMetaData(string title, Version version, IReadOnlyList<string> startupArgs)
    {
        Title            = title;
        Version          = version.ToString();
        StartupArguments = startupArgs;
    }

    public string                Title            { get; }
    public string                Version          { get; }
    public IReadOnlyList<string> StartupArguments { get; }
}
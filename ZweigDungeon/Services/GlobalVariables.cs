using ZweigEngine.Common.Services;

namespace ZweigDungeon.Services;

internal sealed class GlobalVariables : IGlobalVariables
{
    public string   AppTitle        { get; init; } = string.Empty;
    public string   AppIdentifier   { get; init; } = string.Empty;
    public string   AppVersion      { get; init; } = string.Empty;
    public int      VideoWidth      { get; internal set; }
    public int      VideoHeight     { get; internal set; }
    public DateTime FrameClockLocal { get; internal set; }
    public DateTime FrameClockUtc   { get; internal set; }
}
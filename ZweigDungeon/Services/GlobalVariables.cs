using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Platform;

namespace ZweigDungeon.Services;

internal sealed class GlobalVariables : IGlobalVariables
{
    public string           AppTitle           { get; init; } = string.Empty;
    public string           AppIdentifier      { get; init; } = string.Empty;
    public string           AppVersion         { get; init; } = string.Empty;
    public DateTime         FrameClockLocal    { get; internal set; }
    public DateTime         FrameClockUtc      { get; internal set; }
    public TimeSpan         FrameDeltaTime     { get; internal set; }
    public int              ScreenWidth        { get; internal set; }
    public int              ScreenHeight       { get; internal set; }
    public IMouseDevice?    Mouse              { get; internal set; }
    public IKeyboardDevice? Keyboard           { get; internal set; }
}
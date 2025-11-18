using ZweigEngine.Common.Services.Platform;

namespace ZweigEngine.Common.Services;

public interface IGlobalVariables
{
    string           AppTitle        { get; }
    string           AppIdentifier   { get; }
    string           AppVersion      { get; }
    DateTime         FrameClockLocal { get; }
    DateTime         FrameClockUtc   { get; }
    TimeSpan         FrameDeltaTime  { get; }
    int              ScreenWidth     { get; }
    int              ScreenHeight    { get; }
    IMouseDevice?    Mouse           { get; }
    IKeyboardDevice? Keyboard        { get; }
}
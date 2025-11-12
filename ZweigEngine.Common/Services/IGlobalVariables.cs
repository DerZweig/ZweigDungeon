namespace ZweigEngine.Common.Services;

public interface IGlobalVariables
{
    string   AppTitle        { get; }
    string   AppIdentifier   { get; }
    string   AppVersion      { get; }
    DateTime FrameClockLocal { get; }
    DateTime FrameClockUtc   { get; }
    int      VideoWidth      { get; }
    int      VideoHeight     { get; }
}
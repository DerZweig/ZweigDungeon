namespace ZweigEngine.Common.Services;

public interface IGlobalVariables
{
    string   AppTitle          { get; }
    string   AppIdentifier     { get; }
    string   AppVersion        { get; }
    DateTime FrameClockLocal   { get; }
    DateTime FrameClockUtc     { get; }
    TimeSpan FrameDeltaTime    { get; }
    int      VideoWidth        { get; }
    int      VideoHeight       { get; }
    int      MousePositionLeft { get; }
    int      MousePositionTop  { get; }
}
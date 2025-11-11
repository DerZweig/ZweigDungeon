namespace ZweigEngine.Common.Services.Video;

public interface IVideoScreen
{
    IVideoLayer Background { get; }
    IVideoLayer Foreground { get; }
    IVideoLayer Menu       { get; }
}
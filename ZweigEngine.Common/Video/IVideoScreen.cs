namespace ZweigEngine.Common.Video;

public interface IVideoScreen
{
    IVideoLayer Background { get; }
    IVideoLayer Foreground { get; }
}
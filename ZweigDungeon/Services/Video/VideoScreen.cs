using ZweigEngine.Common.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class VideoScreen : IVideoScreen
{
    public VideoScreen(PixelScreen screen)
    {
        Background = new VideoLayer(screen.Background);
        Foreground = new VideoLayer(screen.Foreground);
    }

    public IVideoLayer Background { get; }
    public IVideoLayer Foreground { get; }
}
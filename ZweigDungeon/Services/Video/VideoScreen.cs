using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class VideoScreen : IVideoScreen
{
    public VideoScreen(PixelScreen screen)
    {
        Background = new VideoLayer(screen.Background);
        Foreground = new VideoLayer(screen.Foreground);
        Menu       = new VideoLayer(screen.Menu);
    }
    
    public IVideoLayer Background { get; }
    public IVideoLayer Foreground { get; }
    public IVideoLayer Menu       { get; }
}
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services.Video;

internal sealed class PixelScreen : DisposableObject
{
    public const int Pitch  = 4 * 256;
    public const int Width  = 256;
    public const int Height = 256;

    public PixelScreen()
    {
        Background = new PixelLayer(Width * Height);
        Foreground = new PixelLayer(Width * Height);
    }

    protected override void ReleaseUnmanagedResources()
    {
        Background.Dispose();
        Foreground.Dispose();
    }

    public PixelLayer Background { get; }
    public PixelLayer Foreground { get; }
}
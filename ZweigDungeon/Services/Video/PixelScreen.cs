namespace ZweigDungeon.Services.Video;

internal sealed class PixelScreen
{
    public const int Pitch  = 4 * 256;
    public const int Width  = 256;
    public const int Height = 256;

    public PixelScreen()
    {
        Background = new PixelTarget(Width * Height);
        Foreground = new PixelTarget(Width * Height);
        Menu       = new PixelTarget(Width * Height);
    }

    public PixelTarget Background { get; }
    public PixelTarget Foreground { get; }
    public PixelTarget Menu       { get; }
}
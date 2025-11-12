using ZweigEngine.Common.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class VideoLayer : IVideoLayer
{
    private readonly VideoColor[] m_pixels;

    public VideoLayer(PixelLayer layer)
    {
        m_pixels = layer.Pixels;
    }

    public void Fill(in VideoColor color)
    {
        Array.Fill(m_pixels, color);
    }

    public void PutPixel(int x, int y, in VideoColor color)
    {
        var ny = y % PixelScreen.Height;
        var nx = x % PixelScreen.Width;
        m_pixels[ny * PixelScreen.Width + nx] = color;
    }
}
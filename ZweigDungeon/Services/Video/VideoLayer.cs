using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class VideoLayer : IVideoLayer
{
    private readonly VideoColor[] m_pixels;

    public VideoLayer(PixelTarget target)
    {
        m_pixels = target.Pixels;
    }

    public void Clear()
    {
        Array.Fill(m_pixels, new VideoColor());
    }

    public void PutPixel(ushort x, ushort y, in VideoColor color)
    {
        var ny = y % PixelScreen.Height;
        var nx = x % PixelScreen.Width;
        m_pixels[ny * PixelScreen.Width + nx] = color;
    }
}
using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class ColorBuffer : IColorBuffer
{
    private readonly VideoColor[] m_pixels;

    public ColorBuffer(PixelBuffer buffer)
    {
        m_pixels = buffer.Pixels;
    }

    public void Fill(in VideoColor color)
    {
        Array.Fill(m_pixels, color);
    }

    public void PutPixel(int x, int y, in VideoColor color)
    {
        var ny = y % PixelBuffer.Height;
        var nx = x % PixelBuffer.Width;
        m_pixels[ny * PixelBuffer.Width + nx] = color;
    }
}
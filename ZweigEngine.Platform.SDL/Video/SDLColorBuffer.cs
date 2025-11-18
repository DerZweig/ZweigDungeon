using ZweigEngine.Common.Services.Video;

namespace ZweigEngine.Platform.SDL.Video;

public sealed class SDLColorBuffer : IColorBuffer
{
    private readonly VideoColor[] m_pixels;

    public SDLColorBuffer(SDLPixelBuffer buffer)
    {
        m_pixels = buffer.Pixels;
    }

    public void Fill(in VideoColor color)
    {
        Array.Fill(m_pixels, color);
    }

    public void PutPixel(int x, int y, in VideoColor color)
    {
        var ny = y % SDLPixelBuffer.Height;
        var nx = x % SDLPixelBuffer.Width;
        m_pixels[ny * SDLPixelBuffer.Width + nx] = color;
    }
}
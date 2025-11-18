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
        if (x >= 0 && x < SDLPixelBuffer.Width && y >= 0 && y < SDLPixelBuffer.Height)
        {
            m_pixels[y * SDLPixelBuffer.Width + x] = color;
        }
    }
}
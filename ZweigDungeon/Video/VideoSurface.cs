using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Video;

internal sealed class VideoSurface : DisposableObject
{
    private readonly ushort       m_width;
    private readonly ushort       m_height;
    private readonly VideoColor[] m_pixels;
    private          GCHandle     m_handle;
    private          IntPtr       m_address;

    public VideoSurface(ushort width, ushort height)
    {
        m_width   = width;
        m_height  = height;
        m_pixels  = new VideoColor[width * height];
        m_handle  = GCHandle.Alloc(m_pixels, GCHandleType.Pinned);
        m_address = m_handle.AddrOfPinnedObject();
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (!m_handle.IsAllocated)
        {
            return;
        }

        m_handle.Free();
        m_address = IntPtr.Zero;
    }

    public IntPtr GetAddress()
    {
        return m_address;
    }

    public int GetPitchInBytes()
    {
        return 4 * m_width;
    }

    public void SetPixel(uint x, uint y, byte red, byte green, byte blue, byte alpha)
    {
        var ny = y % m_height;
        var nx = x % m_width;
        m_pixels[ny * m_width + nx] = new VideoColor
        {
            Red   = red,
            Green = green,
            Blue  = blue,
            Alpha = alpha
        };
    }

    public void SetPixel(uint x, uint y, in VideoColor color)
    {
        var ny = y % m_height;
        var nx = x % m_width;
        m_pixels[ny * m_width + nx] = color;
    }
}
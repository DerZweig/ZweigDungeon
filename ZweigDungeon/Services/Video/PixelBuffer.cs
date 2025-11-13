using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Video;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services.Video;

internal sealed class PixelBuffer : DisposableObject
{
    public const int Pitch  = 4 * 256;
    public const int Width  = 256;
    public const int Height = 256;

    private GCHandle m_handle;

    public PixelBuffer()
    {
        var buffer = new VideoColor[Width * Height];
        m_handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        Address  = m_handle.AddrOfPinnedObject();
        Pixels   = buffer;
       
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (!m_handle.IsAllocated)
        {
            return;
        }

        m_handle.Free();
        Address = IntPtr.Zero;
    }

    public IntPtr       Address { get; private set; }
    public VideoColor[] Pixels  { get; }
}
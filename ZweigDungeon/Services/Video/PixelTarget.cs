using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Video;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services.Video;

internal sealed class PixelTarget : DisposableObject
{
    private GCHandle m_handle;

    public PixelTarget(int size)
    {
        var buffer = new VideoColor[size];
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
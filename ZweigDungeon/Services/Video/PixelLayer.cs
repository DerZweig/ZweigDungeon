using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility;
using ZweigEngine.Common.Video;

namespace ZweigDungeon.Services.Video;

internal sealed class PixelLayer : DisposableObject
{
    private GCHandle m_handle;

    public PixelLayer(int size)
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
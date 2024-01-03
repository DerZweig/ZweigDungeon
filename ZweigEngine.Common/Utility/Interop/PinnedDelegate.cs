using System.Runtime.InteropServices;

namespace ZweigEngine.Common.Utility.Interop;

public sealed class PinnedDelegate<TDelegate> : IDisposable where TDelegate : Delegate
{
    private GCHandle m_handle;
    private IntPtr   m_pointer;

    public PinnedDelegate(in TDelegate value, GCHandleType type)
    {
        m_handle  = GCHandle.Alloc(value, type);
        m_pointer = Marshal.GetFunctionPointerForDelegate(value);
    }

    public IntPtr GetAddress()
    {
        return m_handle.IsAllocated ? m_pointer : IntPtr.Zero;
    }

    private void ReleaseUnmanagedResources()
    {
        if (m_handle.IsAllocated)
        {
            m_handle.Free();
        }

        m_pointer = IntPtr.Zero;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~PinnedDelegate()
    {
        ReleaseUnmanagedResources();
    }
}
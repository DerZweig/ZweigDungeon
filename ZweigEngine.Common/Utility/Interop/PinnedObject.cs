using System.Runtime.InteropServices;

namespace ZweigEngine.Common.Utility.Interop;

public sealed class PinnedObject<TObject> : IDisposable
{
    private TObject? m_value;
    private GCHandle m_handle;
    private IntPtr   m_pointer;

    public PinnedObject(in TObject value)
    {
        m_value   = value;
        m_handle  = GCHandle.Alloc(value);
        m_pointer = m_handle.AddrOfPinnedObject();
    }

    public PinnedObject(in TObject value, GCHandleType type)
    {
        m_value   = value;
        m_handle  = GCHandle.Alloc(m_value, type);
        m_pointer = m_handle.AddrOfPinnedObject();
    }

    public TObject GetValue()
    {
        return m_value!;
    }

    public IntPtr GetAddress()
    {
        return m_pointer;
    }

    private void ReleaseUnmanagedResources()
    {
        if (m_handle.IsAllocated)
        {
            m_handle.Free();
            m_pointer = IntPtr.Zero;
        }

        if (m_value is IDisposable disposable)
        {
            disposable.Dispose();
        }

        m_value = default;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~PinnedObject()
    {
        ReleaseUnmanagedResources();
    }
}
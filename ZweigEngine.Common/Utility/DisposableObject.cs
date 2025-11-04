namespace ZweigEngine.Common.Utility;

public abstract class DisposableObject : IDisposable
{
    protected abstract void ReleaseUnmanagedResources();
    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~DisposableObject()
    {
        ReleaseUnmanagedResources();
    }
}
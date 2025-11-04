using SDL3;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Video;

internal sealed class SDLWindow : DisposableObject
{
    private IntPtr m_self;
    
    public SDLWindow(string title, int width, int height)
    {
        m_self = SDL.CreateWindow(title, width, height, 0);
        if (!IsAvailable())
        {
            throw new Exception("Couldn't create SDL window.");
        }
        
        SDL.SetWindowResizable(m_self, true);
        SDL.SetWindowBordered(m_self, true);
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (!IsAvailable())
        {
            return;
        }
        
        SDL.DestroyWindow(m_self);
        m_self = IntPtr.Zero;
    }

    public bool IsAvailable()
    {
        return m_self != IntPtr.Zero;
    }

    public IntPtr GetAddress()
    {
        return m_self;
    }
}
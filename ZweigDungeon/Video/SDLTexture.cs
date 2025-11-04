using SDL3;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Video;

internal sealed class SDLTexture : DisposableObject
{
    private IntPtr m_self;

    public SDLTexture(SDLRenderer renderer, ushort width, ushort height)
    {
        m_self = SDL.CreateTexture(renderer.GetAddress(), SDL.PixelFormat.RGBA8888, SDL.TextureAccess.Streaming, width, height);
        if (!IsAvailable())
        {
            throw new Exception("Couldn't create SDL texture.");
        }
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (IsAvailable())
        {
            SDL.DestroyTexture(m_self);
            m_self = IntPtr.Zero;
        }
    }

    public bool IsAvailable()
    {
        return m_self != IntPtr.Zero;
    }

    public IntPtr GetAddress()
    {
        return m_self;
    }

    public void Upload(VideoSurface surface)
    {
        SDL.UpdateTexture(m_self, IntPtr.Zero, surface.GetAddress(), surface.GetPitchInBytes());
    }
}
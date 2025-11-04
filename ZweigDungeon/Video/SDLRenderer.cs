using SDL3;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Video;

internal sealed class SDLRenderer : DisposableObject
{
    private IntPtr m_self;

    public SDLRenderer(SDLWindow window, string? driverName)
    {
        m_self = SDL.CreateRenderer(window.GetAddress(), driverName);
        if (!IsAvailable())
        {
            throw new Exception("Couldn't create SDL video device.");
        }

        SDL.SetRenderVSync(m_self, 1);
        SDL.SetRenderTextureAddressMode(m_self, SDL.TextureAddressMode.Clamp, SDL.TextureAddressMode.Clamp);
        SDL.SetDefaultTextureScaleMode(m_self, SDL.ScaleMode.Nearest);
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (IsAvailable())
        {
            SDL.DestroyRenderer(m_self);
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

    public bool GetOutputSize(out int width, out int height)
    {
        return SDL.GetRenderOutputSize(m_self, out width, out height);
    }

    public void Clear(byte red, byte green, byte blue, byte alpha)
    {
        SDL.SetRenderDrawColor(m_self, red, green, blue, alpha);
        SDL.RenderClear(m_self);
    }

    public void Present()
    {
        SDL.RenderPresent(m_self);
    }

    public void DrawScreen(SDLTexture texture, int width, int height)
    {
        SDL.RenderTexture(m_self, texture.GetAddress(), new SDL.FRect
        {
            X = 0,
            Y = 0,
            W = width,
            H = height,
        }, IntPtr.Zero);
    }
}
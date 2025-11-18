using ZweigEngine.Common.Services;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Platform.SDL.Video;

internal sealed class SDLWindow : DisposableObject, ISDLEventListener
{
    private const int DefaultWindowWidth  = 800;
    private const int DefaultWindowHeight = 600;
    private const int MinimumOutputWidth  = 640;
    private const int MinimumOutputHeight = 480;

    private readonly SDLPlatformBase        m_platformBase;
    private readonly ILogger           m_logger;
    private readonly SDLLocalVariables m_vars;

    private IntPtr m_window;
    private IntPtr m_renderer;
    private IntPtr m_screen;
    private int    m_videoWidth;
    private int    m_videoHeight;
    private int    m_clientWidth;
    private int    m_clientHeight;
    private int    m_windowTop;
    private int    m_windowLeft;
    private int    m_windowWidth;
    private int    m_windowHeight;

    public SDLWindow(SDLPlatformBase platformBase, ILogger logger, SDLLocalVariables vars)
    {
        m_platformBase = platformBase;
        m_logger  = logger;
        m_vars    = vars;
    }

    protected override void ReleaseUnmanagedResources()
    {
        m_logger.Info(nameof(SDL3.SDL), "Destroy window");
        ReleaseResource(ref m_screen, SDL3.SDL.DestroyTexture);
        ReleaseResource(ref m_renderer, SDL3.SDL.DestroyRenderer);
        ReleaseResource(ref m_window, SDL3.SDL.DestroyWindow);
        m_platformBase.Unsubscribe(this);
    }

    public bool Initialize(string title)
    {
        m_logger.Info(nameof(SDL3.SDL), "Create window");
        if (!SDL3.SDL.CreateWindowAndRenderer(title,
                                              DefaultWindowWidth,
                                              DefaultWindowHeight,
                                              SDL3.SDL.WindowFlags.Hidden,
                                              out m_window,
                                              out m_renderer))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to create window");
            return false;
        }

        if (!SDL3.SDL.SetWindowBordered(m_window, true) ||
            !SDL3.SDL.SetWindowResizable(m_window, true))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to configure window style");
            return false;
        }

        if (!SDL3.SDL.SetRenderVSync(m_renderer, 1) ||
            !SDL3.SDL.SetRenderTextureAddressMode(m_renderer, SDL3.SDL.TextureAddressMode.Clamp,
                                                  SDL3.SDL.TextureAddressMode.Clamp))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to configure renderer");
            return false;
        }

        if (!TryCreateLayerTexture(m_renderer, out m_screen))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to create render target");
            return false;
        }

        if (!SDL3.SDL.SetTextureBlendMode(m_screen, SDL3.SDL.BlendMode.None))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to configure render target");
            return false;
        }

        if (!SDL3.SDL.ShowWindow(m_window))
        {
            m_logger.Error(nameof(SDL3.SDL), "Failed to show window");
            return false;
        }

        m_platformBase.Subscribe(this);
        return true;
    }

    public void InputBegin()
    {
    }

    public void InputMessage(SDL3.SDL.Event ev)
    {
    }

    public void InputFinish()
    {
        if (!SDL3.SDL.GetWindowPosition(m_window, out m_windowLeft, out m_windowTop) ||
            !SDL3.SDL.GetWindowSize(m_window, out m_windowWidth, out m_windowHeight))
        {
            m_logger.Error(nameof(SDL3.SDL), $"Failed to query window size {SDL3.SDL.GetError()}");
            throw new Exception("Failed to update window properties.");
        }

        if (!SDL3.SDL.GetRenderOutputSize(m_renderer, out m_clientWidth, out m_clientHeight))
        {
            m_logger.Error(nameof(SDL3.SDL), $"Failed to query renderer size {SDL3.SDL.GetError()}");
            throw new Exception("Failed to update video properties.");
        }

        m_clientWidth  = Math.Max(m_clientWidth, MinimumOutputWidth);
        m_clientHeight = Math.Max(m_clientHeight, MinimumOutputHeight);

        var scaleWidth  = 1.0f;
        var scaleHeight = 1.0f;
        if (m_clientWidth >= m_clientHeight)
        {
            scaleHeight = (float)m_clientHeight / m_clientWidth;
        }
        else
        {
            scaleWidth = (float)m_clientWidth / m_clientHeight;
        }

        m_videoWidth              = (int)(scaleWidth * SDLPixelBuffer.Width);
        m_videoHeight             = (int)(scaleHeight * SDLPixelBuffer.Height);
        m_vars.WindowVideoWidth   = m_videoWidth;
        m_vars.WindowVideoHeight  = m_videoHeight;
        m_vars.WindowPositionLeft = m_windowLeft;
        m_vars.WindowPositionTop  = m_windowTop;
        m_vars.WindowSizeWidth    = m_windowWidth;
        m_vars.WindowSizeHeight   = m_windowHeight;
        m_vars.WindowClientWidth  = m_clientWidth;
        m_vars.WindowClientHeight = m_clientHeight;
    }

    public void SwapBuffers(in SDLPixelBuffer buffer)
    {
        var srcRect = new SDL3.SDL.FRect { X = 0.0f, Y = 0.0f, W = m_videoWidth, H  = m_videoHeight };
        var dstRect = new SDL3.SDL.FRect { X = 0.0f, Y = 0.0f, W = m_clientWidth, H = m_clientHeight };

        if (!SDL3.SDL.SetRenderDrawColor(m_renderer, 0, 0, 0, 0) ||
            !SDL3.SDL.RenderClear(m_renderer) ||
            !SDL3.SDL.UpdateTexture(m_screen, IntPtr.Zero, buffer.Address, SDLPixelBuffer.Pitch) ||
            !SDL3.SDL.RenderTexture(m_renderer, m_screen, srcRect, dstRect) ||
            !SDL3.SDL.RenderPresent(m_renderer))
        {
            m_logger.Error(nameof(SDL3.SDL), $"Failed to present screen buffer {SDL3.SDL.GetError()}");
            throw new Exception("Failed to present screen buffer");
        }
    }

    private static void ReleaseResource(ref IntPtr ptr, Action<IntPtr> destroy)
    {
        if (ptr == IntPtr.Zero)
        {
            return;
        }

        destroy(ptr);
        ptr = IntPtr.Zero;
    }

    private static bool TryCreateLayerTexture(IntPtr renderer, out IntPtr texture)
    {
        const SDL3.SDL.TextureAccess access = SDL3.SDL.TextureAccess.Streaming;
        const int                    width  = SDLPixelBuffer.Width;
        const int                    height = SDLPixelBuffer.Height;

        var format = BitConverter.IsLittleEndian ? SDL3.SDL.PixelFormat.ABGR8888 : SDL3.SDL.PixelFormat.RGBA8888;

        texture = SDL3.SDL.CreateTexture(renderer, format, access, width, height);
        return texture != IntPtr.Zero && SDL3.SDL.SetTextureScaleMode(texture, SDL3.SDL.ScaleMode.PixelArt);
    }
}
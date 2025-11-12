using SDL3;
using ZweigDungeon.Services.Video;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services.Platform;

internal sealed class SDLDesktop : DisposableObject, IPlatform
{
    private const int DefaultWindowWidth  = 800;
    private const int DefaultWindowHeight = 600;
    private const int MinimumOutputWidth  = 640;
    private const int MinimumOutputHeight = 480;

    private readonly ILogger             m_logger;
    private readonly GlobalVariables     m_globals;
    private readonly IGlobalCancellation m_cancellation;
    private readonly PixelScreen         m_buffers;
    private          bool                m_init;
    private          IntPtr              m_window;
    private          IntPtr              m_renderer;
    private          IntPtr              m_background;
    private          IntPtr              m_foreground;
    private          int                 m_videoWidth;
    private          int                 m_videoHeight;
    private          int                 m_clientWidth;
    private          int                 m_clientHeight;

    public SDLDesktop(ILogger logger, GlobalVariables globals, IGlobalCancellation cancellation, PixelScreen buffers)
    {
        m_logger       = logger;
        m_globals      = globals;
        m_cancellation = cancellation;
        m_buffers      = buffers;
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (!m_init)
        {
            return;
        }

        m_logger.Info(nameof(SDL), "Shutdown");
        ReleaseResource(ref m_foreground, SDL.DestroyTexture);
        ReleaseResource(ref m_background, SDL.DestroyTexture);
        ReleaseResource(ref m_renderer, SDL.DestroyRenderer);
        ReleaseResource(ref m_window, SDL.DestroyWindow);
        SDL.Quit();
    }

    public bool Initialize()
    {
        if (m_init)
        {
            m_logger.Error(nameof(SDL), "Already initialized");
            return false;
        }

        m_logger.Info(nameof(SDL), "Initialize SDL");
        if (!SDL.SetAppMetadata(m_globals.AppTitle, m_globals.AppVersion, m_globals.AppIdentifier) ||
            !SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            m_logger.Error(nameof(SDL), $"Failed to initialize SDL {SDL.GetError()}");
            return false;
        }

        m_init = true;
        m_logger.Info(nameof(SDL), "Creating window");
        if (!SDL.CreateWindowAndRenderer(m_globals.AppTitle,
                                         DefaultWindowWidth,
                                         DefaultWindowHeight,
                                         SDL.WindowFlags.Hidden,
                                         out m_window,
                                         out m_renderer))
        {
            m_logger.Error(nameof(SDL), "Failed to create window");
            return false;
        }

        if (!SDL.SetWindowBordered(m_window, true) ||
            !SDL.SetWindowResizable(m_window, true))
        {
            m_logger.Error(nameof(SDL), "Failed to configure window");
            return false;
        }

        if (!SDL.SetRenderVSync(m_renderer, 1) ||
            !SDL.SetRenderTextureAddressMode(m_renderer, SDL.TextureAddressMode.Clamp, SDL.TextureAddressMode.Clamp))
        {
            m_logger.Error(nameof(SDL), "Failed to configure renderer");
            return false;
        }

        if (!TryCreateLayerTexture(m_renderer, out m_foreground) ||
            !TryCreateLayerTexture(m_renderer, out m_background))
        {
            m_logger.Error(nameof(SDL), "Failed to create render targets");
            return false;
        }

        if (!SDL.SetTextureBlendMode(m_foreground, SDL.BlendMode.Blend) ||
            !SDL.SetTextureBlendMode(m_background, SDL.BlendMode.Blend))
        {
            m_logger.Error(nameof(SDL), "Failed to configure render targets");
            return false;
        }

        if (!SDL.ShowWindow(m_window))
        {
            m_logger.Error(nameof(SDL), "Failed to show window");
            return false;
        }

        m_logger.Info(nameof(SDL), "Initialization completed");
        return true;
    }

    public bool ProcessInput()
    {
        while (true)
        {
            if (m_cancellation.Token.IsCancellationRequested)
            {
                return false;
            }

            if (!SDL.PollEvent(out var e))
            {
                break;
            }

            if (e.Type == (uint)SDL.EventType.Quit)
            {
                m_cancellation.Cancel();
                return false;
            }
        }

        if (!SDL.GetRenderOutputSize(m_renderer, out m_videoWidth, out m_videoHeight))
        {
            m_logger.Error(nameof(SDL), $"Failed to query renderer size {SDL.GetError()}");
        }

        m_videoWidth  = Math.Max(m_videoWidth, MinimumOutputWidth);
        m_videoHeight = Math.Max(m_videoHeight, MinimumOutputHeight);

        var scaleWidth  = 1.0f;
        var scaleHeight = 1.0f;
        if (m_videoWidth >= m_videoHeight)
        {
            scaleHeight = (float)m_videoHeight / m_videoWidth;
        }
        else
        {
            scaleWidth = (float)m_videoWidth / m_videoHeight;
        }

        m_clientWidth        = (int)(scaleWidth * PixelScreen.Width);
        m_clientHeight       = (int)(scaleHeight * PixelScreen.Height);
        m_globals.VideoWidth  = m_clientWidth;
        m_globals.VideoHeight = m_clientHeight;
        return true;
    }

    public void DisplayScreen()
    {
        var srcRect = new SDL.FRect { X = 0.0f, Y = 0.0f, W = m_clientWidth, H = m_clientHeight };
        var dstRect = new SDL.FRect { X = 0.0f, Y = 0.0f, W = m_videoWidth, H  = m_videoHeight };
        
        if (!SDL.SetRenderDrawColor(m_renderer, 0, 0, 0, 0) ||
            !SDL.RenderClear(m_renderer) ||
            !UploadLayerToTexture(m_background, m_buffers.Background) ||
            !UploadLayerToTexture(m_foreground, m_buffers.Foreground) ||
            !SDL.RenderTexture(m_renderer, m_background, srcRect, dstRect) ||
            !SDL.RenderTexture(m_renderer, m_foreground, srcRect, dstRect) ||
            !SDL.RenderPresent(m_renderer))
        {
            m_logger.Error(nameof(SDL), $"Failed to present screen {SDL.GetError()}");
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

    private static bool UploadLayerToTexture(IntPtr texture, PixelLayer layer)
    {
        return SDL.UpdateTexture(texture, IntPtr.Zero, layer.Address, PixelScreen.Pitch);
    }

    private static bool TryCreateLayerTexture(IntPtr renderer, out IntPtr texture)
    {
        const SDL.TextureAccess access = SDL.TextureAccess.Streaming;
        const int               width  = PixelScreen.Width;
        const int               height = PixelScreen.Height;

        var format = BitConverter.IsLittleEndian ? SDL.PixelFormat.ABGR8888 : SDL.PixelFormat.RGBA8888;

        texture = SDL.CreateTexture(renderer, format, access, width, height);
        return texture != IntPtr.Zero && SDL.SetTextureScaleMode(texture, SDL.ScaleMode.PixelArt);
    }
}
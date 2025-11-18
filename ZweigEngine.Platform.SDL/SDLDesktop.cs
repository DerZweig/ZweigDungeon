using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Platform;
using ZweigEngine.Common.Services.ServiceProvider;
using ZweigEngine.Common.Services.Video;
using ZweigEngine.Common.Utility;
using ZweigEngine.Platform.SDL.Video;

namespace ZweigEngine.Platform.SDL;

public class SDLDesktop : DisposableObject, IPlatformProvider
{
    private readonly SDLPlatformBase   m_base;
    private readonly SDLLocalVariables m_vars;
    private readonly SDLWindow         m_window;
    private readonly SDLKeyboard       m_keyboard;
    private readonly SDLMouse          m_mouse;
    private readonly SDLPixelBuffer    m_screen;
    private readonly SDLColorBuffer    m_buffer;

    public SDLDesktop(ILogger logger)
    {
        m_vars     = new SDLLocalVariables();
        m_base     = new SDLPlatformBase(logger);
        m_window   = new SDLWindow(m_base, logger, m_vars);
        m_keyboard = new SDLKeyboard(m_base, logger, m_vars);
        m_mouse    = new SDLMouse(m_base, logger, m_vars);
        m_screen   = new SDLPixelBuffer();
        m_buffer   = new SDLColorBuffer(m_screen);
    }

    protected override void ReleaseUnmanagedResources()
    {
        m_screen.Dispose();
        m_window.Dispose();
        m_keyboard.Dispose();
        m_mouse.Dispose();
        m_base.Dispose();
    }

    public int              VideoWidth  => m_vars.WindowVideoWidth;
    public int              VideoHeight => m_vars.WindowVideoHeight;
    public IColorBuffer     VideoBuffer => m_buffer;
    public IKeyboardDevice? Keyboard    => m_vars.Keyboard;
    public IMouseDevice?    Mouse       => m_vars.Mouse;

    public bool Start(string title, string version, string identifier)
    {
        if (!m_base.Initialize(title, version, identifier))
        {
            return false;
        }

        if (!m_window.Initialize(title) || !m_keyboard.Initialize() || !m_mouse.Initialize())
        {
            return false;
        }

        return true;
    }

    public bool ProcessInput(CancellationToken cancellationToken)
    {
        return m_base.ProcessInput(cancellationToken);
    }

    public void SwapBuffers()
    {
        m_window.SwapBuffers(m_screen);
    }
}
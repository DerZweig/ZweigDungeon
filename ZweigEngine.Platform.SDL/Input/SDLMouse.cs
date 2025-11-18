using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Platform;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLMouse : DisposableObject, ISDLEventListener
{
    private const int SDL_BUTTON_LEFT   = 1;
    private const int SDL_BUTTON_MIDDLE = 2;
    private const int SDL_BUTTON_RIGHT  = 3;
    private const int SDL_BUTTON_X1     = 4;
    private const int SDL_BUTTON_X2     = 5;

    private readonly SDLPlatformBase        m_platformBase;
    private readonly ILogger           m_logger;
    private readonly SDLLocalVariables m_vars;
    private readonly SDLMouseState        m_device;

    public SDLMouse(SDLPlatformBase platformBase, ILogger logger, SDLLocalVariables vars)
    {
        m_platformBase = platformBase;
        m_logger  = logger;
        m_vars    = vars;
        m_device  = new SDLMouseState();
    }

    protected override void ReleaseUnmanagedResources()
    {
        m_logger.Info(nameof(SDL3.SDL), "Destroy mouse");
        SetEnabled(false);
        m_platformBase.Unsubscribe(this);
    }

    public bool Initialize()
    {
        m_logger.Info(nameof(SDL3.SDL), "Create mouse");
        m_platformBase.Subscribe(this);
        SetEnabled(SDL3.SDL.HasMouse());
        return true;
    }

    public void InputBegin()
    {
        SetEnabled(SDL3.SDL.HasMouse());
        m_device.Clear(true, false);
    }

    public void InputMessage(SDL3.SDL.Event ev)
    {
        switch (ev.Type)
        {
            case (uint)SDL3.SDL.EventType.MouseButtonDown:
            case (uint)SDL3.SDL.EventType.MouseButtonUp:
                switch (ev.Button.Button)
                {
                    case SDL_BUTTON_LEFT:
                        m_device.SetPulsed(MouseButton.Left, true);
                        break;
                    case SDL_BUTTON_RIGHT:
                        m_device.SetPulsed(MouseButton.Right, true);
                        break;
                    case SDL_BUTTON_MIDDLE:
                        m_device.SetPulsed(MouseButton.Middle, true);
                        break;
                    case SDL_BUTTON_X1:
                        m_device.SetPulsed(MouseButton.Button4, true);
                        break;
                    case SDL_BUTTON_X2:
                        m_device.SetPulsed(MouseButton.Button5, true);
                        break;
                }

                break;
            case (uint)SDL3.SDL.EventType.WindowFocusGained:
            case (uint)SDL3.SDL.EventType.WindowFocusLost:
                m_device.Clear(true, true);
                break;
        }
    }

    public void InputFinish()
    {
        var buttons = SDL3.SDL.GetGlobalMouseState(out var x, out var y);
        var posX    = (int)x - m_vars.WindowPositionLeft;
        var posY    = (int)y - m_vars.WindowPositionTop;

        if (posX >= 0 && posX <= m_vars.WindowSizeWidth &&
            posY >= 0 && posY <= m_vars.WindowSizeHeight &&
            m_vars.WindowClientWidth != 0 && m_vars.WindowClientHeight != 0)
        {
            var scaleX = (float)posX / m_vars.WindowClientWidth;
            var scaleY = (float)posY / m_vars.WindowClientHeight;

            m_device.IsInsideScreen = true;
            m_device.PositionLeft   = (int)(scaleX * m_vars.WindowVideoWidth);
            m_device.PositionTop    = (int)(scaleY * m_vars.WindowVideoHeight);
        }
        else
        {
            m_device.IsInsideScreen = false;
        }

        m_device.SetPressed(MouseButton.Left, (buttons & SDL3.SDL.MouseButtonFlags.Left) != 0);
        m_device.SetPressed(MouseButton.Right, (buttons & SDL3.SDL.MouseButtonFlags.Right) != 0);
        m_device.SetPressed(MouseButton.Middle, (buttons & SDL3.SDL.MouseButtonFlags.Middle) != 0);
        m_device.SetPressed(MouseButton.Button4, (buttons & SDL3.SDL.MouseButtonFlags.X1) != 0);
        m_device.SetPressed(MouseButton.Button5, (buttons & SDL3.SDL.MouseButtonFlags.X2) != 0);
    }

    private void SetEnabled(bool value)
    {
        if (value && m_vars.Mouse != m_device)
        {
            m_logger.Info(nameof(SDL3.SDL), "Activate mouse");
            m_vars.Mouse = m_device;
        }
        else if (!value && m_vars.Mouse == m_device)
        {
            m_logger.Info(nameof(SDL3.SDL), "Deactivate mouse");
            m_vars.Mouse = null;
        }
    }
}
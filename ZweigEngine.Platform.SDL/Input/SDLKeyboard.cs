using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Platform;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLKeyboard : DisposableObject, ISDLEventListener
{
    private readonly SDLPlatformBase                                m_platformBase;
    private readonly ILogger                                   m_logger;
    private readonly SDLLocalVariables                         m_vars;
    private readonly SDLKeyboardState                             m_device;
    private readonly Dictionary<SDL3.SDL.Keycode, KeyboardKey> m_mapper;

    public SDLKeyboard(SDLPlatformBase platformBase, ILogger logger, SDLLocalVariables vars)
    {
        m_platformBase = platformBase;
        m_logger  = logger;
        m_vars    = vars;
        m_device  = new SDLKeyboardState();
        m_mapper = new Dictionary<SDL3.SDL.Keycode, KeyboardKey>
        {
            [SDL3.SDL.Keycode.Space]        = KeyboardKey.Space,
            [SDL3.SDL.Keycode.Apostrophe]   = KeyboardKey.Apostrophe,
            [SDL3.SDL.Keycode.Comma]        = KeyboardKey.Comma,
            [SDL3.SDL.Keycode.Minus]        = KeyboardKey.Minus,
            [SDL3.SDL.Keycode.Period]       = KeyboardKey.Period,
            [SDL3.SDL.Keycode.Slash]        = KeyboardKey.Slash,
            [SDL3.SDL.Keycode.Alpha0]       = KeyboardKey.N0,
            [SDL3.SDL.Keycode.Alpha1]       = KeyboardKey.N1,
            [SDL3.SDL.Keycode.Alpha2]       = KeyboardKey.N2,
            [SDL3.SDL.Keycode.Alpha3]       = KeyboardKey.N3,
            [SDL3.SDL.Keycode.Alpha4]       = KeyboardKey.N4,
            [SDL3.SDL.Keycode.Alpha5]       = KeyboardKey.N5,
            [SDL3.SDL.Keycode.Alpha6]       = KeyboardKey.N6,
            [SDL3.SDL.Keycode.Alpha7]       = KeyboardKey.N7,
            [SDL3.SDL.Keycode.Alpha8]       = KeyboardKey.N8,
            [SDL3.SDL.Keycode.Alpha9]       = KeyboardKey.N9,
            [SDL3.SDL.Keycode.Semicolon]    = KeyboardKey.Semicolon,
            [SDL3.SDL.Keycode.Equals]       = KeyboardKey.Equal,
            [SDL3.SDL.Keycode.A]            = KeyboardKey.A,
            [SDL3.SDL.Keycode.B]            = KeyboardKey.B,
            [SDL3.SDL.Keycode.C]            = KeyboardKey.C,
            [SDL3.SDL.Keycode.D]            = KeyboardKey.D,
            [SDL3.SDL.Keycode.E]            = KeyboardKey.E,
            [SDL3.SDL.Keycode.F]            = KeyboardKey.F,
            [SDL3.SDL.Keycode.G]            = KeyboardKey.G,
            [SDL3.SDL.Keycode.H]            = KeyboardKey.H,
            [SDL3.SDL.Keycode.I]            = KeyboardKey.I,
            [SDL3.SDL.Keycode.J]            = KeyboardKey.J,
            [SDL3.SDL.Keycode.K]            = KeyboardKey.K,
            [SDL3.SDL.Keycode.L]            = KeyboardKey.L,
            [SDL3.SDL.Keycode.M]            = KeyboardKey.M,
            [SDL3.SDL.Keycode.N]            = KeyboardKey.N,
            [SDL3.SDL.Keycode.O]            = KeyboardKey.O,
            [SDL3.SDL.Keycode.P]            = KeyboardKey.P,
            [SDL3.SDL.Keycode.Q]            = KeyboardKey.Q,
            [SDL3.SDL.Keycode.R]            = KeyboardKey.R,
            [SDL3.SDL.Keycode.S]            = KeyboardKey.S,
            [SDL3.SDL.Keycode.T]            = KeyboardKey.T,
            [SDL3.SDL.Keycode.U]            = KeyboardKey.U,
            [SDL3.SDL.Keycode.V]            = KeyboardKey.V,
            [SDL3.SDL.Keycode.W]            = KeyboardKey.W,
            [SDL3.SDL.Keycode.X]            = KeyboardKey.X,
            [SDL3.SDL.Keycode.Y]            = KeyboardKey.Y,
            [SDL3.SDL.Keycode.Z]            = KeyboardKey.Z,
            [SDL3.SDL.Keycode.LeftBracket]  = KeyboardKey.LeftBracket,
            [SDL3.SDL.Keycode.Backslash]    = KeyboardKey.Backslash,
            [SDL3.SDL.Keycode.RightBracket] = KeyboardKey.RightBracket,
            [SDL3.SDL.Keycode.Grave]        = KeyboardKey.GraveAccent,
            [SDL3.SDL.Keycode.Escape]       = KeyboardKey.Escape,
            [SDL3.SDL.Keycode.Return]       = KeyboardKey.Return,
            [SDL3.SDL.Keycode.Tab]          = KeyboardKey.Tab,
            [SDL3.SDL.Keycode.Backspace]    = KeyboardKey.Backspace,
            [SDL3.SDL.Keycode.Insert]       = KeyboardKey.Insert,
            [SDL3.SDL.Keycode.Delete]       = KeyboardKey.Delete,
            [SDL3.SDL.Keycode.Right]        = KeyboardKey.Right,
            [SDL3.SDL.Keycode.Left]         = KeyboardKey.Left,
            [SDL3.SDL.Keycode.Down]         = KeyboardKey.Down,
            [SDL3.SDL.Keycode.Up]           = KeyboardKey.Up,
            [SDL3.SDL.Keycode.Pageup]       = KeyboardKey.PageUp,
            [SDL3.SDL.Keycode.Pagedown]     = KeyboardKey.PageDown,
            [SDL3.SDL.Keycode.Home]         = KeyboardKey.Home,
            [SDL3.SDL.Keycode.End]          = KeyboardKey.End,
            [SDL3.SDL.Keycode.Capslock]     = KeyboardKey.CapsLock,
            [SDL3.SDL.Keycode.ScrollLock]   = KeyboardKey.ScrollLock,
            [SDL3.SDL.Keycode.NumLockClear] = KeyboardKey.NumLock,
            [SDL3.SDL.Keycode.PrintScreen]  = KeyboardKey.PrintScreen,
            [SDL3.SDL.Keycode.Pause]        = KeyboardKey.Pause,
            [SDL3.SDL.Keycode.F1]           = KeyboardKey.F1,
            [SDL3.SDL.Keycode.F2]           = KeyboardKey.F2,
            [SDL3.SDL.Keycode.F3]           = KeyboardKey.F3,
            [SDL3.SDL.Keycode.F4]           = KeyboardKey.F4,
            [SDL3.SDL.Keycode.F5]           = KeyboardKey.F5,
            [SDL3.SDL.Keycode.F6]           = KeyboardKey.F6,
            [SDL3.SDL.Keycode.F7]           = KeyboardKey.F7,
            [SDL3.SDL.Keycode.F8]           = KeyboardKey.F8,
            [SDL3.SDL.Keycode.F9]           = KeyboardKey.F9,
            [SDL3.SDL.Keycode.F10]          = KeyboardKey.F10,
            [SDL3.SDL.Keycode.F11]          = KeyboardKey.F11,
            [SDL3.SDL.Keycode.F12]          = KeyboardKey.F12,
            [SDL3.SDL.Keycode.F13]          = KeyboardKey.F13,
            [SDL3.SDL.Keycode.F14]          = KeyboardKey.F14,
            [SDL3.SDL.Keycode.F15]          = KeyboardKey.F15,
            [SDL3.SDL.Keycode.F16]          = KeyboardKey.F16,
            [SDL3.SDL.Keycode.F17]          = KeyboardKey.F17,
            [SDL3.SDL.Keycode.F18]          = KeyboardKey.F18,
            [SDL3.SDL.Keycode.F19]          = KeyboardKey.F19,
            [SDL3.SDL.Keycode.F20]          = KeyboardKey.F20,
            [SDL3.SDL.Keycode.F21]          = KeyboardKey.F21,
            [SDL3.SDL.Keycode.F22]          = KeyboardKey.F22,
            [SDL3.SDL.Keycode.F23]          = KeyboardKey.F23,
            [SDL3.SDL.Keycode.F24]          = KeyboardKey.F24,
            [SDL3.SDL.Keycode.Kp0]          = KeyboardKey.Keypad0,
            [SDL3.SDL.Keycode.Kp1]          = KeyboardKey.Keypad1,
            [SDL3.SDL.Keycode.Kp2]          = KeyboardKey.Keypad2,
            [SDL3.SDL.Keycode.Kp3]          = KeyboardKey.Keypad3,
            [SDL3.SDL.Keycode.Kp4]          = KeyboardKey.Keypad4,
            [SDL3.SDL.Keycode.Kp5]          = KeyboardKey.Keypad5,
            [SDL3.SDL.Keycode.Kp6]          = KeyboardKey.Keypad6,
            [SDL3.SDL.Keycode.Kp7]          = KeyboardKey.Keypad7,
            [SDL3.SDL.Keycode.Kp8]          = KeyboardKey.Keypad8,
            [SDL3.SDL.Keycode.Kp9]          = KeyboardKey.Keypad9,
            [SDL3.SDL.Keycode.KpDecimal]    = KeyboardKey.KeypadDecimal,
            [SDL3.SDL.Keycode.KpDivide]     = KeyboardKey.KeypadDivide,
            [SDL3.SDL.Keycode.KpMultiply]   = KeyboardKey.KeypadMultiply,
            [SDL3.SDL.Keycode.KpMinus]      = KeyboardKey.KeypadSubtract,
            [SDL3.SDL.Keycode.KpPlus]       = KeyboardKey.KeypadAdd,
            [SDL3.SDL.Keycode.KpEnter]      = KeyboardKey.KeypadEnter,
            [SDL3.SDL.Keycode.KpEquals]     = KeyboardKey.KeypadEqual,
            [SDL3.SDL.Keycode.LShift]       = KeyboardKey.LeftShift,
            [SDL3.SDL.Keycode.RShift]       = KeyboardKey.RightShift,
            [SDL3.SDL.Keycode.LCtrl]        = KeyboardKey.LeftControl,
            [SDL3.SDL.Keycode.RCtrl]        = KeyboardKey.RightControl,
            [SDL3.SDL.Keycode.LAlt]         = KeyboardKey.LeftAlt,
            [SDL3.SDL.Keycode.LGUI]         = KeyboardKey.LeftWindows,
            [SDL3.SDL.Keycode.RAlt]         = KeyboardKey.RightAlt,
            [SDL3.SDL.Keycode.RGUI]         = KeyboardKey.RightWindows
        };
    }

    protected override void ReleaseUnmanagedResources()
    {
        m_logger.Info(nameof(SDL3.SDL), "Destroy keyboard");
        m_device.Clear(true, true);
        m_platformBase.Unsubscribe(this);
    }

    public bool Initialize()
    {
        m_logger.Info(nameof(SDL3.SDL), "Create keyboard");
        m_platformBase.Subscribe(this);

        return true;
    }

    public void InputBegin()
    {
        m_device.Clear(true, false);
        SetEnabled(SDL3.SDL.HasKeyboard());
    }

    public void InputMessage(SDL3.SDL.Event ev)
    {
        switch (ev.Type)
        {
            case (uint)SDL3.SDL.EventType.KeyDown:
            case (uint)SDL3.SDL.EventType.KeyUp:
            {
                var key = m_mapper.GetValueOrDefault(ev.Key.Key, KeyboardKey.Unknown);
                m_device.SetPulsed(key, true);
                m_device.SetPressed(key, ev.Type == (uint)SDL3.SDL.EventType.KeyDown);
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
    }

    private void SetEnabled(bool value)
    {
        if (value && m_vars.Keyboard != m_device)
        {
            m_logger.Info(nameof(SDL3.SDL), "Activate keyboard");
            m_vars.Keyboard = m_device;
        }
        else if (!value && m_vars.Keyboard == m_device)
        {
            m_logger.Info(nameof(SDL3.SDL), "Deactivate keyboard");
            m_vars.Keyboard = null;
        }
    }
}
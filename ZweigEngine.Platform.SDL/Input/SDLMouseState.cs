using ZweigEngine.Common.Services.Platform;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLMouseState : IMouseDevice
{
    private const uint BUTTON_LEFT_BIT   = 1;
    private const uint BUTTON_RIGHT_BIT  = 1 << 1;
    private const uint BUTTON_MIDDLE_BIT = 1 << 2;
    private const uint BUTTON_EXTRA1_BIT = 1 << 3;
    private const uint BUTTON_EXTRA2_BIT = 1 << 4;

    private uint m_pressed;
    private uint m_pulsed;

    public bool IsInsideScreen { get; internal set; }

    public int PositionLeft { get; internal set; }

    public int PositionTop { get; internal set; }

    public bool IsButtonPressed(MouseButton button)
    {
        return (m_pressed & GetButtonBits(button)) != 0;
    }

    public bool IsButtonReleased(MouseButton button)
    {
        return (m_pressed & GetButtonBits(button)) == 0;
    }

    public bool IsButtonPulsed(MouseButton button)
    {
        return (m_pulsed & GetButtonBits(button)) != 0;
    }

    internal void SetPressed(MouseButton button, bool value)
    {
        if (value)
        {
            m_pressed |= GetButtonBits(button);
        }
        else
        {
            m_pressed &= ~GetButtonBits(button);
        }
    }

    internal void SetPulsed(MouseButton button, bool value)
    {
        if (value)
        {
            m_pulsed |= GetButtonBits(button);
        }
        else
        {
            m_pulsed &= ~GetButtonBits(button);
        }
    }

    internal void Clear(bool pulsed, bool pressed)
    {
        m_pulsed  = pulsed ? 0u : m_pulsed;
        m_pressed = pressed ? 0u : m_pressed;
    }

    private static uint GetButtonBits(MouseButton button)
    {
        var bits = button switch
                   {
                       MouseButton.Left => BUTTON_LEFT_BIT,
                       MouseButton.Right => BUTTON_RIGHT_BIT,
                       MouseButton.Middle => BUTTON_MIDDLE_BIT,
                       MouseButton.Button4 => BUTTON_EXTRA1_BIT,
                       MouseButton.Button5 => BUTTON_EXTRA2_BIT,
                       _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
                   };
        return bits;
    }
}
using ZweigEngine.Common.Services.Platform;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLKeyboardState : IKeyboardDevice
{
    private const uint KeyboardTablesSize = 0x01FFu;

    private readonly bool[] m_pressed;
    private readonly bool[] m_pulsed;

    public SDLKeyboardState()
    {
        m_pressed = new bool[KeyboardTablesSize + 1];
        m_pulsed  = new bool[KeyboardTablesSize + 1];
    }

    public bool IsKeyPressed(KeyboardKey key)
    {
        return m_pressed[(int)key];
    }

    public bool IsKeyReleased(KeyboardKey key)
    {
        return key != KeyboardKey.Unknown && !m_pressed[(int)key];
    }

    public bool IsKeyPulsed(KeyboardKey key)
    {
        return key != KeyboardKey.Unknown && m_pulsed[(int)key];
    }

    internal void SetPressed(KeyboardKey key, bool value)
    {
        if (key != KeyboardKey.Unknown)
        {
            m_pressed[(int)key] = value;
        }
    }

    internal void SetPulsed(KeyboardKey key, bool value)
    {
        if (key != KeyboardKey.Unknown)
        {
            m_pulsed[(int)key] = value;
        }
    }

    internal void Clear(bool pulsed, bool pressed)
    {
        if (pressed)
        {
            Array.Fill(m_pressed, false);
        }

        if (pulsed)
        {
            Array.Fill(m_pulsed, false);
        }
    }
}
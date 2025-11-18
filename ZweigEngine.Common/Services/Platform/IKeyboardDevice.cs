namespace ZweigEngine.Common.Services.Platform;

public interface IKeyboardDevice
{
    bool IsKeyPressed(KeyboardKey key);
    bool IsKeyReleased(KeyboardKey key);
    bool IsKeyPulsed(KeyboardKey key);
}
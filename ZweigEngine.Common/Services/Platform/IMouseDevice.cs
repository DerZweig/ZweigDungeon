namespace ZweigEngine.Common.Services.Platform;

public interface IMouseDevice
{
    bool IsInsideScreen { get; }
    int  PositionLeft   { get; }
    int  PositionTop    { get; }
    bool IsButtonPressed(MouseButton button);
    bool IsButtonReleased(MouseButton button);
    bool IsButtonPulsed(MouseButton button);
}
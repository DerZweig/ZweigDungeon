using ZweigEngine.Common.Services.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Services.Interfaces.Platform;

public delegate void PlatformKeyPressedDelegate(IPlatformKeyboard keyboard, KeyboardKey key);

public delegate void PlatformKeyReleasedDelegate(IPlatformKeyboard keyboard, KeyboardKey key);

public delegate void PlatformKeyTypedDelegate(IPlatformKeyboard keyboard, char character);

public interface IPlatformKeyboard
{
	event PlatformKeyPressedDelegate  OnKeyPressed;
	event PlatformKeyReleasedDelegate OnKeyReleased;
	event PlatformKeyTypedDelegate    OnKeyTyped;

	bool IsKeyPressed(KeyboardKey key);
	bool IsKeyReleased(KeyboardKey key);
}
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Services.Interfaces.Platform.Messages;

public interface IKeyboardListener
{
	void KeyPressed(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyReleased(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyTyped(IPlatformKeyboard keyboard, char character);
}
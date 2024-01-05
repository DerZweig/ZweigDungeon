using ZweigEngine.Common.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Interfaces.Platform.Messages;

public interface IKeyboardListener
{
	void KeyPressed(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyReleased(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyTyped(IPlatformKeyboard keyboard, char character);
}
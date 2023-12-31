using ZweigDungeon.Common.Interfaces.Platform.Constants;

namespace ZweigDungeon.Common.Interfaces.Platform.Messages;

public interface IKeyboardListener
{
	void KeyPressed(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyReleased(IPlatformKeyboard keyboard, KeyboardKey key);
	void KeyTyped(IPlatformKeyboard keyboard, char character);
}
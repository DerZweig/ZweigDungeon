using ZweigDungeon.Common.Constants;

namespace ZweigDungeon.Common.Interfaces.Platform;

public interface IPlatformKeyboard
{
	bool IsKeyPressed(KeyboardKey key);
	bool IsKeyReleased(KeyboardKey key);
}
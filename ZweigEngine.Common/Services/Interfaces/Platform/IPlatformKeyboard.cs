using ZweigEngine.Common.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Interfaces.Platform;

public interface IPlatformKeyboard
{
	bool IsKeyPressed(KeyboardKey key);
	bool IsKeyReleased(KeyboardKey key);
}
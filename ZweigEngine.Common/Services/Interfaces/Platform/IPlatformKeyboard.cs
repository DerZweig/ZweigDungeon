using ZweigEngine.Common.Services.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Services.Interfaces.Platform;

public interface IPlatformKeyboard
{
	bool IsKeyPressed(KeyboardKey key);
	bool IsKeyReleased(KeyboardKey key);
}
using ZweigEngine.Common.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Interfaces.Platform;

public interface IPlatformMouse
{
	int  GetPositionLeft();
	int  GetPositionTop();
	bool IsButtonPressed(MouseButton button);
	bool IsButtonReleased(MouseButton button);
}
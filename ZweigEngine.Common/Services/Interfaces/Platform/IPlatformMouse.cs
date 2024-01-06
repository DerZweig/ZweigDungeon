using ZweigEngine.Common.Services.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Services.Interfaces.Platform;

public interface IPlatformMouse
{
	int  GetPositionLeft();
	int  GetPositionTop();
	bool IsButtonPressed(MouseButton button);
	bool IsButtonReleased(MouseButton button);
}
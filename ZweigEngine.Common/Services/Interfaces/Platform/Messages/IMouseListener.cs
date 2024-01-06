using ZweigEngine.Common.Services.Interfaces.Platform.Constants;

namespace ZweigEngine.Common.Services.Interfaces.Platform.Messages;

public interface IMouseListener
{
	void MouseMoved(IPlatformMouse mouse, int left, int top);
	void ButtonPressed(IPlatformMouse mouse, MouseButton button);
	void ButtonReleased(IPlatformMouse mouse, MouseButton button);
	void ScrolledHorizontal(IPlatformMouse mouse, int offset);
	void ScrolledVertical(IPlatformMouse mouse, int offset);
}
using ZweigDungeon.Common.Constants;

namespace ZweigDungeon.Common.Interfaces.Platform.Messages;

public interface IMouseListener
{
	void MouseMoved(IPlatformMouse mouse, int left, int top);
	void ButtonPressed(IPlatformMouse mouse, MouseButton button);
	void ButtonReleased(IPlatformMouse mouse, MouseButton button);
	void ScrolledHorizontal(IPlatformMouse mouse, int offset);
	void ScrolledVertical(IPlatformMouse mouse, int offset);
}
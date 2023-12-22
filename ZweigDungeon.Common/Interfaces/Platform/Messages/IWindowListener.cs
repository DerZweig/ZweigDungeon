namespace ZweigDungeon.Common.Interfaces.Platform.Messages;

public interface IWindowListener
{
	void WindowCreated(IPlatformWindow window);
	void WindowClosing(IPlatformWindow window);
	void WindowBeginFrame(IPlatformWindow window);
	void WindowFinishFrame(IPlatformWindow window);
	void WinodwUpdateFrame(IPlatformWindow window);
}
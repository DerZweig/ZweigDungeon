namespace ZweigDungeon.Common.Interfaces.Platform.Messages;

public interface IWindowListener
{
	void WindowCreated(IPlatformWindow window);
	void WindowClosing(IPlatformWindow window);
	void WindowUpdateFrame(IPlatformWindow window);
}
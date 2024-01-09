namespace ZweigEngine.Common.Services.Interfaces.Platform;

public delegate void PlatformWindowDelegate(IPlatformWindow window);

public interface IPlatformWindow
{
	event PlatformWindowDelegate OnCreated;
	event PlatformWindowDelegate OnClosing;
	event PlatformWindowDelegate OnUpdate;
	
	bool IsAvailable();
	bool IsFocused();
	int  GetPositionLeft();
	int  GetPositionTop();
	int  GetSizeWidth();
	int  GetSizeHeight();
	int  GetViewportWidth();
	int  GetViewportHeight();

	void Show();
	void Close();
	void Create();
	void Update();
	
	void SetTitle(string text);
	void SetStyle(bool bordered, bool resizable);
	void SetPosition(int left, int top);
	void SetSize(int width, int height);
	void SetMinimumSize(int width, int height);
	void SetMaximumSize(int width, int height);
}
using ZweigEngine.Native.Win32.Constants;

namespace ZweigEngine.Native.Win32;

internal interface IWin32WindowComponent
{
	void OnAttach();
	void OnDetach();
	void OnBeginUpdate();
	void OnFinishUpdate();
	void OnMessage(long lTime, IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam);
}
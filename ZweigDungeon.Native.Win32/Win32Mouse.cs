using ZweigDungeon.Common.Constants;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Native.Win32.Constants;

namespace ZweigDungeon.Native.Win32;

public sealed class Win32Mouse : IDisposable, IPlatformMouse, IWin32WindowComponent
{
	private readonly MessageBus  m_messageBus;
	private readonly Win32Window m_window;

	public Win32Mouse(MessageBus messageBus, Win32Window window)
	{
		m_messageBus  = messageBus;
		m_window = window;
		m_window.AddComponent(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.RemoveComponent(this);
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Win32Mouse()
	{
		ReleaseUnmanagedResources();
	}

	public int GetPositionLeft()
	{
		return 0;
	}

	public int GetPositionTop()
	{
		return 0;
	}

	public bool IsButtonPressed(MouseButton button)
	{
		return false;
	}

	public bool IsButtonReleased(MouseButton button)
	{
		return false;
	}

	void IWin32WindowComponent.OnAttach()
	{
	}

	void IWin32WindowComponent.OnDetach()
	{
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
	}

	void IWin32WindowComponent.OnMessage(long lTime, nint hWindow, Win32MessageType uMessage, nint wParam, nint lParam)
	{
	}
}
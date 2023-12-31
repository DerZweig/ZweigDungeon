using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Constants;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Native.Win32.Constants;

namespace ZweigDungeon.Native.Win32;

public sealed class Win32Mouse : IDisposable, IPlatformMouse, IWin32WindowComponent
{
	private readonly MessageBus  m_messageBus;
	private readonly Win32Window m_window;
	private readonly bool[]      m_buttonStates;
	private          int         m_positionLeft;
	private          int         m_positionTop;

	public Win32Mouse(MessageBus messageBus, Win32Window window)
	{
		m_messageBus   = messageBus;
		m_window       = window;
		m_buttonStates = new bool[8];
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
		return m_positionLeft;
	}

	public int GetPositionTop()
	{
		return m_positionTop;
	}

	public bool IsButtonPressed(MouseButton button)
	{
		switch (button)
		{
			case MouseButton.Button1:
				return m_buttonStates[0];
			case MouseButton.Button2:
				return m_buttonStates[1];
			case MouseButton.Button3:
				return m_buttonStates[2];
			case MouseButton.Button4:
				return m_buttonStates[3];
			case MouseButton.Button5:
				return m_buttonStates[4];
			default:
				throw new ArgumentOutOfRangeException(nameof(button), button, null);
		}
	}

	public bool IsButtonReleased(MouseButton button)
	{
		switch (button)
		{
			case MouseButton.Button1:
				return !m_buttonStates[0];
			case MouseButton.Button2:
				return !m_buttonStates[1];
			case MouseButton.Button3:
				return !m_buttonStates[2];
			case MouseButton.Button4:
				return !m_buttonStates[3];
			case MouseButton.Button5:
				return !m_buttonStates[4];
			default:
				throw new ArgumentOutOfRangeException(nameof(button), button, null);
		}
	}

	void IWin32WindowComponent.OnAttach()
	{
		Array.Fill(m_buttonStates, false);
		m_positionLeft = 0;
		m_positionTop  = 0;
	}

	void IWin32WindowComponent.OnDetach()
	{
		Array.Fill(m_buttonStates, false);
		m_positionLeft = 0;
		m_positionTop  = 0;
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
	}

	void IWin32WindowComponent.OnMessage(long lTime, IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam)
	{
		switch (uMessage)
		{
			case Win32MessageType.MouseMove:
				TranslateMove(lParam);
				break;
			case Win32MessageType.LeftButtonDown:
			case Win32MessageType.MiddleButtonDown:
			case Win32MessageType.RightButtonDown:
			case Win32MessageType.ExtraButtonDown:
			case Win32MessageType.LeftButtonUp:
			case Win32MessageType.MiddleButtonUp:
			case Win32MessageType.RightButtonUp:
			case Win32MessageType.ExtraButtonUp:
				TranslateMove(lParam);
				UpdateButtons(wParam);
				break;
			case Win32MessageType.MouseWheelVertical:
				TranslateMove(lParam);
				TranslateVerticalScroll(wParam);
				break;
			case Win32MessageType.MouseWheelHorizontal:
				TranslateMove(lParam);
				TranslateHorizontalScroll(wParam);
				break;
			case Win32MessageType.SetFocus:
			case Win32MessageType.KillFocus:
				ClearButtons();
				break;
		}
	}

	private void ClearButtons()
	{
		TranslateButton(0, false, MouseButton.Button1);
		TranslateButton(1, false, MouseButton.Button2);
		TranslateButton(2, false, MouseButton.Button3);
		TranslateButton(3, false, MouseButton.Button4);
		TranslateButton(4, false, MouseButton.Button5);
	}

	private void TranslateMove(IntPtr lParam)
	{
		var value = (ulong)lParam.ToInt64();
		var top   = (short)(value >> 16);
		var left  = (short)(value & 0xFFFFu);

		if (left != m_positionLeft || top != m_positionTop)
		{
			m_positionLeft = left;
			m_positionTop  = top;
			m_messageBus.Broadcast<IMouseListener>(listener => listener.MouseMoved(this, left, top));
		}
	}

	private void TranslateButton(int index, bool state, MouseButton button)
	{
		var current = m_buttonStates[index];
		var changed = state != current;

		m_buttonStates[index] = state;

		if (state && changed)
		{
			m_messageBus.Broadcast<IMouseListener>(listener => listener.ButtonPressed(this, button));
		}
		else if (changed)
		{
			m_messageBus.Broadcast<IMouseListener>(listener => listener.ButtonReleased(this, button));
		}
	}

	private void TranslateVerticalScroll(IntPtr wParam)
	{
		var value  = (ulong)wParam.ToInt64();
		var offset = (short)(value >> 16);
		m_messageBus.Broadcast<IMouseListener>(listener => listener.ScrolledVertical(this, offset));
	}

	private void TranslateHorizontalScroll(IntPtr wParam)
	{
		var value  = (ulong)wParam.ToInt64();
		var offset = (short)(value >> 16);
		m_messageBus.Broadcast<IMouseListener>(listener => listener.ScrolledHorizontal(this, offset));
	}

	private void UpdateButtons(IntPtr wParam)
	{
		var flags  = (Win32MouseKey)wParam.ToInt64();
		var left   = (flags & Win32MouseKey.LeftButton) != 0;
		var right  = (flags & Win32MouseKey.RightButton) != 0;
		var middle = (flags & Win32MouseKey.MiddleButton) != 0;
		var extra1 = (flags & Win32MouseKey.ExtraButton1) != 0;
		var extra2 = (flags & Win32MouseKey.ExtraButton2) != 0;

		TranslateButton(0, left, MouseButton.Button1);
		TranslateButton(1, right, MouseButton.Button2);
		TranslateButton(2, middle, MouseButton.Button3);
		TranslateButton(3, extra1, MouseButton.Button4);
		TranslateButton(4, extra2, MouseButton.Button5);
	}
}
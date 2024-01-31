using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;

namespace ZweigEngine.Native.Win32;

public sealed class Win32Mouse : IDisposable, IPlatformMouse, IWin32WindowComponent
{
	private readonly Win32Window m_window;
	private readonly bool[]      m_buttonStates;
	private          int         m_positionLeft;
	private          int         m_positionTop;

	public Win32Mouse(Win32Window window)
	{
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

	public event PlatformMouseMovedDelegate?    OnMouseMoved;
	public event PlatformMousePressedDelegate?  OnMousePressed;
	public event PlatformMouseReleasedDelegate? OnMouseReleased;
	public event PlatformMouseScrolledDelegate? OnMouseScrolledHorizontal;
	public event PlatformMouseScrolledDelegate? OnMouseScrolledVertical;

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
		return button switch
		       {
			       MouseButton.Button1 => m_buttonStates[0],
			       MouseButton.Button2 => m_buttonStates[1],
			       MouseButton.Button3 => m_buttonStates[2],
			       MouseButton.Button4 => m_buttonStates[3],
			       MouseButton.Button5 => m_buttonStates[4],
			       _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
		       };
	}

	public bool IsButtonReleased(MouseButton button)
	{
		return button switch
		       {
			       MouseButton.Button1 => !m_buttonStates[0],
			       MouseButton.Button2 => !m_buttonStates[1],
			       MouseButton.Button3 => !m_buttonStates[2],
			       MouseButton.Button4 => !m_buttonStates[3],
			       MouseButton.Button5 => !m_buttonStates[4],
			       _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
		       };
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
			OnMouseMoved?.Invoke(this, left, top);
		}
	}

	private void TranslateButton(int index, bool state, MouseButton button)
	{
		var current = m_buttonStates[index];
		var changed = state != current;

		m_buttonStates[index] = state;

		if (state && changed)
		{
			OnMousePressed?.Invoke(this, m_positionLeft, m_positionTop, button);
		}
		else if (changed)
		{
			OnMouseReleased?.Invoke(this, m_positionLeft, m_positionTop, button);
		}
	}

	private void TranslateVerticalScroll(IntPtr wParam)
	{
		var value  = (ulong)wParam.ToInt64();
		var offset = (short)(value >> 16);
		OnMouseScrolledVertical?.Invoke(this, m_positionLeft, m_positionTop, offset);
	}

	private void TranslateHorizontalScroll(IntPtr wParam)
	{
		var value  = (ulong)wParam.ToInt64();
		var offset = (short)(value >> 16);
		OnMouseScrolledHorizontal?.Invoke(this, m_positionLeft, m_positionTop, offset);
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
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Common.Services.Interfaces.Platform.Messages;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Messages;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Prototypes;

namespace ZweigEngine.Native.Win32;

public sealed class Win32Keyboard : IDisposable, IPlatformKeyboard, IWin32WindowComponent
{
	private const uint KEYBOARD_PRESSED_STATE = 0x8000u;
	private const uint KEYBOARD_TABLES_SIZE   = 0x01FFu;

	private static readonly KeyboardKey[] g_mapper;
	private static readonly KeyboardKey[] g_allKeys;

	private readonly MessageBus  m_messageBus;
	private readonly Win32Window m_window;
	private readonly bool[]      m_currentStates;

	// ReSharper disable InconsistentNaming
	private readonly PfnMapVirtualKeyDelegate MapVirtualKey;
	private readonly PfnGetKeyStateDelegate   GetAsyncKeyState;
	private readonly PfnGetKeyStateDelegate   GetKeyState;
	// ReSharper restore InconsistentNaming

	private long        m_lastTime;
	private bool        m_lastState;
	private KeyboardKey m_lastKey;

	static Win32Keyboard()
	{
		g_mapper  = new KeyboardKey[KEYBOARD_TABLES_SIZE + 1];
		g_allKeys = Enum.GetValues(typeof(KeyboardKey)).Cast<KeyboardKey>().ToArray();

		Array.Fill(g_mapper, KeyboardKey.Unknown);
		g_mapper[0x00B] = KeyboardKey.D0;
		g_mapper[0x002] = KeyboardKey.D1;
		g_mapper[0x003] = KeyboardKey.D2;
		g_mapper[0x004] = KeyboardKey.D3;
		g_mapper[0x005] = KeyboardKey.D4;
		g_mapper[0x006] = KeyboardKey.D5;
		g_mapper[0x007] = KeyboardKey.D6;
		g_mapper[0x008] = KeyboardKey.D7;
		g_mapper[0x009] = KeyboardKey.D8;
		g_mapper[0x00A] = KeyboardKey.D9;
		g_mapper[0x01E] = KeyboardKey.A;
		g_mapper[0x030] = KeyboardKey.B;
		g_mapper[0x02E] = KeyboardKey.C;
		g_mapper[0x020] = KeyboardKey.D;
		g_mapper[0x012] = KeyboardKey.E;
		g_mapper[0x021] = KeyboardKey.F;
		g_mapper[0x022] = KeyboardKey.G;
		g_mapper[0x023] = KeyboardKey.H;
		g_mapper[0x017] = KeyboardKey.I;
		g_mapper[0x024] = KeyboardKey.J;
		g_mapper[0x025] = KeyboardKey.K;
		g_mapper[0x026] = KeyboardKey.L;
		g_mapper[0x032] = KeyboardKey.M;
		g_mapper[0x031] = KeyboardKey.N;
		g_mapper[0x018] = KeyboardKey.O;
		g_mapper[0x019] = KeyboardKey.P;
		g_mapper[0x010] = KeyboardKey.Q;
		g_mapper[0x013] = KeyboardKey.R;
		g_mapper[0x01F] = KeyboardKey.S;
		g_mapper[0x014] = KeyboardKey.T;
		g_mapper[0x016] = KeyboardKey.U;
		g_mapper[0x02F] = KeyboardKey.V;
		g_mapper[0x011] = KeyboardKey.W;
		g_mapper[0x02D] = KeyboardKey.X;
		g_mapper[0x015] = KeyboardKey.Y;
		g_mapper[0x02C] = KeyboardKey.Z;
		g_mapper[0x028] = KeyboardKey.Apostrophe;
		g_mapper[0x02B] = KeyboardKey.Backslash;
		g_mapper[0x033] = KeyboardKey.Comma;
		g_mapper[0x00D] = KeyboardKey.Equal;
		g_mapper[0x029] = KeyboardKey.GraveAccent;
		g_mapper[0x01A] = KeyboardKey.LeftBracket;
		g_mapper[0x00C] = KeyboardKey.Minus;
		g_mapper[0x034] = KeyboardKey.Period;
		g_mapper[0x01B] = KeyboardKey.RightBracket;
		g_mapper[0x027] = KeyboardKey.Semicolon;
		g_mapper[0x035] = KeyboardKey.Slash;
		g_mapper[0x00E] = KeyboardKey.Backspace;
		g_mapper[0x153] = KeyboardKey.Delete;
		g_mapper[0x14F] = KeyboardKey.End;
		g_mapper[0x01C] = KeyboardKey.Enter;
		g_mapper[0x001] = KeyboardKey.Escape;
		g_mapper[0x147] = KeyboardKey.Home;
		g_mapper[0x152] = KeyboardKey.Insert;
		g_mapper[0x15D] = KeyboardKey.Menu;
		g_mapper[0x151] = KeyboardKey.PageDown;
		g_mapper[0x149] = KeyboardKey.PageUp;
		g_mapper[0x045] = KeyboardKey.Pause;
		g_mapper[0x146] = KeyboardKey.Pause;
		g_mapper[0x039] = KeyboardKey.Space;
		g_mapper[0x00F] = KeyboardKey.Tab;
		g_mapper[0x03A] = KeyboardKey.CapsLock;
		g_mapper[0x145] = KeyboardKey.NumLock;
		g_mapper[0x046] = KeyboardKey.ScrollLock;
		g_mapper[0x03B] = KeyboardKey.Func1;
		g_mapper[0x03C] = KeyboardKey.Func2;
		g_mapper[0x03D] = KeyboardKey.Func3;
		g_mapper[0x03E] = KeyboardKey.Func4;
		g_mapper[0x03F] = KeyboardKey.Func5;
		g_mapper[0x040] = KeyboardKey.Func6;
		g_mapper[0x041] = KeyboardKey.Func7;
		g_mapper[0x042] = KeyboardKey.Func8;
		g_mapper[0x043] = KeyboardKey.Func9;
		g_mapper[0x044] = KeyboardKey.Func10;
		g_mapper[0x057] = KeyboardKey.Func11;
		g_mapper[0x058] = KeyboardKey.Func12;
		g_mapper[0x064] = KeyboardKey.Func13;
		g_mapper[0x065] = KeyboardKey.Func14;
		g_mapper[0x066] = KeyboardKey.Func15;
		g_mapper[0x067] = KeyboardKey.Func16;
		g_mapper[0x068] = KeyboardKey.Func17;
		g_mapper[0x069] = KeyboardKey.Func18;
		g_mapper[0x06A] = KeyboardKey.Func19;
		g_mapper[0x06B] = KeyboardKey.Func20;
		g_mapper[0x06C] = KeyboardKey.Func21;
		g_mapper[0x06D] = KeyboardKey.Func22;
		g_mapper[0x06E] = KeyboardKey.Func23;
		g_mapper[0x076] = KeyboardKey.Func24;
		g_mapper[0x038] = KeyboardKey.LeftAlt;
		g_mapper[0x01D] = KeyboardKey.LeftControl;
		g_mapper[0x02A] = KeyboardKey.LeftShift;
		g_mapper[0x15B] = KeyboardKey.LeftWindows;
		g_mapper[0x137] = KeyboardKey.PrintScreen;
		g_mapper[0x138] = KeyboardKey.RightAlt;
		g_mapper[0x11D] = KeyboardKey.RightControl;
		g_mapper[0x036] = KeyboardKey.RightShift;
		g_mapper[0x15C] = KeyboardKey.RightWindows;
		g_mapper[0x150] = KeyboardKey.Down;
		g_mapper[0x14B] = KeyboardKey.Left;
		g_mapper[0x14D] = KeyboardKey.Right;
		g_mapper[0x148] = KeyboardKey.Up;
		g_mapper[0x052] = KeyboardKey.Keypad0;
		g_mapper[0x04F] = KeyboardKey.Keypad1;
		g_mapper[0x050] = KeyboardKey.Keypad2;
		g_mapper[0x051] = KeyboardKey.Keypad3;
		g_mapper[0x04B] = KeyboardKey.Keypad4;
		g_mapper[0x04C] = KeyboardKey.Keypad5;
		g_mapper[0x04D] = KeyboardKey.Keypad6;
		g_mapper[0x047] = KeyboardKey.Keypad7;
		g_mapper[0x048] = KeyboardKey.Keypad8;
		g_mapper[0x049] = KeyboardKey.Keypad9;
		g_mapper[0x04E] = KeyboardKey.KeypadAdd;
		g_mapper[0x053] = KeyboardKey.KeypadDecimal;
		g_mapper[0x135] = KeyboardKey.KeypadDivide;
		g_mapper[0x11C] = KeyboardKey.KeypadEnter;
		g_mapper[0x059] = KeyboardKey.KeypadEqual;
		g_mapper[0x037] = KeyboardKey.KeypadMultiply;
		g_mapper[0x04A] = KeyboardKey.KeypadSubtract;
	}

	public Win32Keyboard(NativeLibraryLoader libraryLoader, MessageBus messageBus, Win32Window window)
	{
		m_messageBus    = messageBus;
		m_window        = window;
		m_currentStates = new bool[KEYBOARD_TABLES_SIZE];

		libraryLoader.LoadFunction("user32", "MapVirtualKeyW", out MapVirtualKey);
		libraryLoader.LoadFunction("user32", "GetAsyncKeyState", out GetAsyncKeyState);
		libraryLoader.LoadFunction("user32", "GetKeyState", out GetKeyState);
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

	~Win32Keyboard()
	{
		ReleaseUnmanagedResources();
	}

	public bool IsKeyPressed(KeyboardKey key)
	{
		return false;
	}

	public bool IsKeyReleased(KeyboardKey key)
	{
		return false;
	}

	void IWin32WindowComponent.OnAttach()
	{
		m_lastKey = KeyboardKey.Unknown;
		Array.Fill(m_currentStates, false);
	}

	void IWin32WindowComponent.OnDetach()
	{
		m_lastKey = KeyboardKey.Unknown;
		Array.Fill(m_currentStates, false);
	}

	void IWin32WindowComponent.OnBeginUpdate()
	{
		FlushPending();
		UpdateAsyncReleased();
	}

	void IWin32WindowComponent.OnFinishUpdate()
	{
	}

	void IWin32WindowComponent.OnMessage(long lTime, IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam)
	{
		switch (uMessage)
		{
			case Win32MessageType.KeyDown:
			case Win32MessageType.SysKeyDown:
				TranslateKeyChange(true, lTime, wParam, lParam);
				UpdateAsyncReleased();
				break;
			case Win32MessageType.KeyUp:
			case Win32MessageType.SysKeyUp:
				TranslateKeyChange(false, lTime, wParam, lParam);
				UpdateAsyncReleased();
				break;
			case Win32MessageType.Char:
				TranslateKeyTyped(wParam);
				break;
			case Win32MessageType.SetFocus:
			case Win32MessageType.KillFocus:
				Clear();
				break;
			default:
				FlushPending();
				UpdateAsyncReleased();
				break;
		}
	}

	private void Clear()
	{
		FlushPending();

		foreach (var key in g_allKeys)
		{
			UpdateState(false, key);
		}
	}

	private void FlushPending()
	{
		if (m_lastKey == KeyboardKey.Unknown)
		{
			return;
		}

		UpdateState(m_lastState, m_lastKey);
		m_lastKey = KeyboardKey.Unknown;
	}

	private void UpdateAsyncReleased()
	{
		QueryAsyncReleased(Win32VirtualKey.LeftShift, KeyboardKey.LeftShift);
		QueryAsyncReleased(Win32VirtualKey.RightShift, KeyboardKey.RightShift);
		QueryAsyncReleased(Win32VirtualKey.LeftWindows, KeyboardKey.LeftWindows);
		QueryAsyncReleased(Win32VirtualKey.RightWindows, KeyboardKey.RightWindows);
		QueryAsyncReleased(Win32VirtualKey.LeftControl, KeyboardKey.LeftControl);
		QueryAsyncReleased(Win32VirtualKey.RightControl, KeyboardKey.RightControl);
	}

	private void QueryAsyncReleased(Win32VirtualKey virtualKey, KeyboardKey key)
	{
		var state   = GetAsyncKeyState(virtualKey);
		var pressed = state & KEYBOARD_PRESSED_STATE;
		if (pressed == 0)
		{
			UpdateState(false, key);
		}
	}

	private KeyboardKey ReadScanCode(IntPtr wParam, IntPtr lParam)
	{
		var value  = (ulong)lParam.ToInt64();
		var higher = (ushort)(value >> 16);
		var code   = higher & KEYBOARD_TABLES_SIZE;

		if (code != 0)
		{
			return g_mapper[code];
		}

		var vkey = (uint)wParam.ToInt64();
		var scan = MapVirtualKey(vkey, Win32MapVirtualKeyType.KeyCodeToScanCode);

		if (scan < KEYBOARD_TABLES_SIZE)
		{
			return g_mapper[scan];
		}

		return KeyboardKey.Unknown;
	}

	private void TranslateKeyChange(bool pressed, long lTime, IntPtr wParam, IntPtr lParam)
	{
		var key  = ReadScanCode(wParam, lParam);
		var vkey = (Win32VirtualKey)(wParam.ToInt64() & 0xFFFF);

		switch (vkey)
		{
			case Win32VirtualKey.ProcessKey:
				FlushPending();
				break;
			case Win32VirtualKey.Print:
				FlushPending();
				UpdateState(true, KeyboardKey.PrintScreen);
				UpdateState(false, KeyboardKey.PrintScreen);
				break;
			case Win32VirtualKey.Shift:
			case Win32VirtualKey.LeftWindows:
			case Win32VirtualKey.RightWindows:
				FlushPending();
				m_lastTime  = lTime;
				m_lastState = pressed;
				m_lastKey   = key;
				break;
			default:
				if (m_lastTime != lTime ||
				    m_lastState != pressed ||
				    m_lastKey != KeyboardKey.LeftControl)
				{
					FlushPending();
				}

				m_lastTime  = lTime;
				m_lastState = pressed;
				m_lastKey   = key;
				break;
		}
	}

	private void TranslateKeyTyped(IntPtr wParam)
	{
		m_messageBus.Broadcast<IKeyboardListener>(listener => listener.KeyTyped(this, (char)wParam.ToInt64()));
	}

	private void UpdateState(bool pressed, KeyboardKey key)
	{
		if (key == KeyboardKey.Unknown)
		{
			return;
		}
		
		var index   = (int)key;
		var state   = m_currentStates[index];
		var changed = state != pressed;

		m_currentStates[index] = pressed;
		if (changed)
		{
			if (pressed)
			{
				m_messageBus.Broadcast<IKeyboardListener>(listener => listener.KeyPressed(this, key));
			}
			else
			{
				m_messageBus.Broadcast<IKeyboardListener>(listener => listener.KeyReleased(this, key));
			}
		}
	}
}
using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.Constants;

namespace ZweigEngine.Native.Win32.Structures;

internal delegate IntPtr PfnWindowProc(IntPtr hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam);

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct Win32WindowClassEx
{
	public int              Size;
	public Win32ClassStyles Styles;
	public IntPtr           WindowProc;
	public int              ClassExtraBytes;
	public int              WindowExtraBytes;
	public IntPtr           InstanceHandle;
	public IntPtr           IconHandle;
	public IntPtr           CursorHandle;
	public IntPtr           BackgroundBrushHandle;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string MenuName;

	[MarshalAs(UnmanagedType.LPWStr)]
	public string ClassName;

	public IntPtr SmallIconHandle;
}
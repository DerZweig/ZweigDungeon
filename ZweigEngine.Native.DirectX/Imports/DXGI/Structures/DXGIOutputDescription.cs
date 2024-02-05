using System.Runtime.InteropServices;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;
using ZweigEngine.Native.DirectX.Imports.Structures;

namespace ZweigEngine.Native.DirectX.Imports.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGIOutputDescription
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
	public string DeviceName;

	public DXGIRectangle    DesktopCoordinates;
	public Win32Bool        AttachedToDesktop;
	public DXGIModeRotation ModeRotation;
	public IntPtr           MonitorHandle;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DXGIRectangle
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}
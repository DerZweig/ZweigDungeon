using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGIOutputDescription
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
	public string DeviceName;

	public Win32Rectangle   DesktopCoordinates;
	public Win32Bool        AttachedToDesktop;
	public DXGIModeRotation ModeRotation;
	public IntPtr           MonitorHandle;
}
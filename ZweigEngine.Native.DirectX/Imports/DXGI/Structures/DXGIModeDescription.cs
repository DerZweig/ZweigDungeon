using System.Runtime.InteropServices;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;

namespace ZweigEngine.Native.DirectX.Imports.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGIModeDescription
{
	public uint                  Width;
	public uint                  Height;
	public DXGIRational          RefreshRate;
	public DXGIFormat            Format;
	public DXGIModeScanlineOrder ScanlineOrdering;
	public DXGIModeScaling       Scaling;
}
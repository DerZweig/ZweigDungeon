using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("ae02eedb-c735-4690-8d52-5a8dc20213aa")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGIOutputMethodTable
{
	public DXGIObjectMethodTable Super;
	public IntPtr                GetDesc;
	public IntPtr                GetDisplayModeList;
	public IntPtr                FindClosestMatchingMode;
	public IntPtr                WaitForVBlank;
	public IntPtr                TakeOwnership;
	public IntPtr                ReleaseOwnership;
	public IntPtr                GetGammaControlCapabilities;
	public IntPtr                SetGammaControl;
	public IntPtr                GetGammaControl;
	public IntPtr                SetDisplaySurface;
	public IntPtr                GetDisplaySurfaceData;
	public IntPtr                GetFrameStatistics;
}
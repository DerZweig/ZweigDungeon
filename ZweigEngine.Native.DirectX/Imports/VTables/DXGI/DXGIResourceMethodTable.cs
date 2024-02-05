using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("035f3ab4-482e-4e50-b41f-8a7f8bd8960b")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGIResourceMethodTable
{
	public DXGIDeviceSubObjectMethodTable Super;
	public IntPtr                         GetSharedHandle;
	public IntPtr                         GetUsage;
	public IntPtr                         SetEvictionPriority;
	public IntPtr                         GetEvictionPriority;
}
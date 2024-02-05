using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("3d3e0379-f9de-4d58-bb6c-18d62992f1a6")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGIDeviceSubObjectMethodTable
{
	public DXGIObjectMethodTable Super;
	public IntPtr                GetDevice;
}
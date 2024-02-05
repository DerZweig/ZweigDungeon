using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11ResourceMethodTable
{
	public D3D11DeviceChildMethodTable Super;
	public IntPtr                      GetResourceType;
	public IntPtr                      SetEvictionPriority;
	public IntPtr                      GetEvictionPriority;
}
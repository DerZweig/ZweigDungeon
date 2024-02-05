using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("839d1216-bb2e-412b-b7f4-a9dbebe08ed1")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11ViewMethodTable
{
	public D3D11DeviceChildMethodTable Super;
	public IntPtr                      GetResource;
}
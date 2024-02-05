using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("dfdba067-0b8d-4865-875b-d7b4516cc164")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11RenderTargetViewMethodTable
{
	public D3D11ViewMethodTable Super;
	public IntPtr               GetDec;
}
using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11Texture2DMethodTable
{
	public D3D11ResourceMethodTable Super;
	public IntPtr                   GetDec;
}
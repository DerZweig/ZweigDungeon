using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("1841e5c8-16b0-489b-bcc8-44cfb0d5deae")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11DeviceChildMethodTable
{
	public UnknownMethodTable Super;
	public IntPtr             GetDevice;
	public IntPtr             GetPrivateData;
	public IntPtr             SetPrivateData;
	public IntPtr             SetPrivateDataInterface;
}
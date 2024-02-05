using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("7b7166ec-21c7-44ae-b21a-c9ae321ae369")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGIFactoryMethodTable
{
	public DXGIObjectMethodTable Super;
	public IntPtr                EnumAdapters;
	public IntPtr                MakeWindowAssociation;
	public IntPtr                GetWindowAssociation;
	public IntPtr                CreateSwapChain;
	public IntPtr                CreateSoftwareAdapter;
}
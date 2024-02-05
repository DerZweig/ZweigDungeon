using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGIAdapterDescription
{
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
	public string Description;

	public uint    VendorId;
	public uint    DeviceId;
	public uint    SubSysId;
	public uint    Revision;
	public UIntPtr DedicatedVideoMemory;
	public UIntPtr DedicatedSystemMemory;
	public UIntPtr SharedSystemMemory;
	public ulong   AdapterLuid;
}
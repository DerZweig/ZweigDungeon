using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGIDeviceMethodTable
{
	public DXGIObjectMethodTable Super;
	public IntPtr                GetAdapter;
	public IntPtr                CreateSurface;
	public IntPtr                QueryResourceResidency;
	public IntPtr                SetGPUThreadPriority;
	public IntPtr                GetGPUThreadPriority;
}
using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

[Guid("310d36a0-d2e7-4c0a-aa04-6a9d23b8886a")]
[StructLayout(LayoutKind.Sequential)]
internal struct DXGISwapChainMethodTable
{
	public DXGIDeviceSubObjectMethodTable Super;
	public IntPtr                         Present;
	public IntPtr                         GetBuffer;
	public IntPtr                         SetFullscreenState;
	public IntPtr                         GetFullscreenState;
	public IntPtr                         GetDesc;
	public IntPtr                         ResizeBuffers;
	public IntPtr                         ResizeTarget;
	public IntPtr                         GetContainingOutput;
	public IntPtr                         GetFrameStatistics;
	public IntPtr                         GetLastPresentCount;
}
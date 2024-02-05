using System.Runtime.InteropServices;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;
using ZweigEngine.Native.DirectX.Imports.Structures;

namespace ZweigEngine.Native.DirectX.Imports.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGISwapChainDescription
{
	public DXGIModeDescription   BufferDescription;
	public DXGISampleDescription SampleDescription;
	public DXGIUsage             BufferUsage;
	public uint                  BufferCount;
	public IntPtr                OutputWindow;
	public Win32Bool             Windowed;
	public DXGISwapEffect        SwapEffect;
	public uint                  Flags;
}
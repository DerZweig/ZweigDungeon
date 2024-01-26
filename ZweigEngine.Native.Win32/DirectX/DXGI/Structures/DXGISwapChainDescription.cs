using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI.Structures;

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
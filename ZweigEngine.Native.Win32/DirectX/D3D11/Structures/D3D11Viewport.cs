using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.DirectX.D3D11.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct D3D11Viewport
{
	public float TopLeftX;
	public float TopLeftY;
	public float Width;
	public float Height;
	public float MinDepth;
	public float MaxDepth;
}
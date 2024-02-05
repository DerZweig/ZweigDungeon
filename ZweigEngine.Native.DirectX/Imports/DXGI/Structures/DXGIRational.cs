using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.DXGI.Structures;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct DXGIRational
{
	public uint Numerator;
	public uint Denominator;
}
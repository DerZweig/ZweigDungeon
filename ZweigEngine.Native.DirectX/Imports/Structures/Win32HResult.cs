using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32HResult
{
	private int code;

	public bool Success => code >= 0;
	public bool Failure => code < 0;
}
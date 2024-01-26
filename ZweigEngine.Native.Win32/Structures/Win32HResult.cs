using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct Win32HResult
{
	private int code;

	public bool Success => code >= 0;
	public bool Failure => code < 0;
}
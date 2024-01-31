using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32Message
{
    public IntPtr       Hwnd;
    public uint       Value;
    public IntPtr       WParam;
    public IntPtr       LParam;
    public uint       Time;
    public Win32Point Point;
}
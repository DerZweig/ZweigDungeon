using System.Runtime.InteropServices;

namespace ZweigDungeon.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32Message
{
    public nint       Hwnd;
    public uint       Value;
    public nint       WParam;
    public nint       LParam;
    public uint       Time;
    public Win32Point Point;
}
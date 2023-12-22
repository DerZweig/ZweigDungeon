using System.Runtime.InteropServices;

namespace ZweigDungeon.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32Point
{
    public int X;
    public int Y;
}
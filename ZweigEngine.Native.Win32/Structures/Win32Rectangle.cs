using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32Rectangle
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
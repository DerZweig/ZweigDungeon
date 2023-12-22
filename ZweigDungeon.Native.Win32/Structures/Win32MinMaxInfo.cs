using System.Runtime.InteropServices;

namespace ZweigDungeon.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32MinMaxInfo
{
    private readonly Win32Point Reserved;
    public           Win32Point MaxSize;
    public           Win32Point MaxPosition;
    public           Win32Point MinTrackSize;
    public           Win32Point MaxTrackSize;
}
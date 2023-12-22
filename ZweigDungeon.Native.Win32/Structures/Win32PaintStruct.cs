using System.Runtime.InteropServices;

namespace ZweigDungeon.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32PaintStruct
{
    public nint           DeviceContext;
    public bool           Erase;
    public Win32Rectangle PaintRectangle;
    public bool           Restore;
    public bool           IncrmentUpdate;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] rgbReserved;
}
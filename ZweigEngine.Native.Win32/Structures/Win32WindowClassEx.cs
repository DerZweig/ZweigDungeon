using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.Constants;

namespace ZweigEngine.Native.Win32.Structures;

internal delegate nint PfnWindowProc(nint hWindow, Win32MessageType uMessage, IntPtr wParam, IntPtr lParam);

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct Win32WindowClassEx
{
    public int              Size;
    public Win32ClassStyles Styles;
    public nint             WindowProc;
    public int              ClassExtraBytes;
    public int              WindowExtraBytes;
    public nint             InstanceHandle;
    public nint             IconHandle;
    public nint             CursorHandle;
    public nint             BackgroundBrushHandle;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string MenuName;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string ClassName;

    public nint SmallIconHandle;
}
namespace ZweigDungeon.Native.Win32.Constants;

[Flags]
internal enum Win32ClassStyles
{
    HorizontalRedraw = 0x0002,
    OwnDeviceContext = 0x0020,
    VerticalRedraw   = 0x0001
}
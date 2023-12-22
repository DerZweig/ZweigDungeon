namespace ZweigDungeon.Native.Win32.Constants;

[Flags]
internal enum Win32PixelFormatDescriptorFlags : uint
{
    Doublebuffer  = 0x00000001,
    DrawToWindow  = 0x00000004,
    SupportOpenGL = 0x00000020
}
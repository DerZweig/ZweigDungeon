namespace ZweigDungeon.Native.Win32.Constants;

[Flags]
internal enum Win32SetWindowPositionCommands : uint
{
    FrameChanged = 0x0020,
    NoActivate   = 0x0010,
    NoCopyBits   = 0x0100,
    NoMove       = 0x0002,
    NoSize       = 0x0001,
    NoZOrder     = 0x0004
}
namespace ZweigDungeon.Native.Win32.Constants;

[Flags]
internal enum Win32MouseKey : uint
{
    LeftButton   = 0x0001,
    RightButton  = 0x0002,
    MiddleButton = 0x0008,
    ExtraButton1 = 0x0020,
    ExtraButton2 = 0x0040
}
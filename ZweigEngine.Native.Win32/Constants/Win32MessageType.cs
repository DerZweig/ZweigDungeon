namespace ZweigEngine.Native.Win32.Constants;

internal enum Win32MessageType : uint
{
    Move                 = 0x0003,
    Size                 = 0x0005,
    SetFocus             = 0x0007,
    KillFocus            = 0x0008,
    Paint                = 0x000F,
    ShowWindow           = 0x0018,
    KeyDown              = 0x0100,
    KeyUp                = 0x0101,
    Char                 = 0x0102,
    SysChar              = 0x0106,
    SysKeyDown           = 0x0104,
    SysKeyUp             = 0x0105,
    MouseMove            = 0x0200,
    LeftButtonDown       = 0x0201,
    LeftButtonUp         = 0x0202,
    RightButtonDown      = 0x0204,
    RightButtonUp        = 0x0205,
    MiddleButtonDown     = 0x0207,
    MiddleButtonUp       = 0x0208,
    MouseWheelVertical   = 0x020A,
    ExtraButtonDown      = 0x020B,
    ExtraButtonUp        = 0x020C,
    MouseWheelHorizontal = 0x020E,
    EnterSizeMove        = 0x0231,
    ExitSizeMove         = 0x0232,
    Close                = 0x0010,
    GetMinMaxInfo        = 0x0024
}
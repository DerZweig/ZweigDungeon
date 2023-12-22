namespace ZweigDungeon.Native.Win32.Constants;

[Flags]
internal enum Win32QueueStatusFlags
{
    Allinput    = Input | Postmessage | Timer | Paint | Hotkey | Sendmessage,
    Hotkey      = 0x0080,
    Input       = Mouse | Key | Rawinput,
    Key         = 0x0001,
    Mouse       = Mousemove | Mousebutton,
    Mousebutton = 0x0004,
    Mousemove   = 0x0002,
    Paint       = 0x0020,
    Postmessage = 0x0008,
    Rawinput    = 0x0400,
    Sendmessage = 0x0040,
    Timer       = 0x0010
}
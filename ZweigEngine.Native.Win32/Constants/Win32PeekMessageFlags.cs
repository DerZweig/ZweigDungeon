namespace ZweigEngine.Native.Win32.Constants;

[Flags]
internal enum Win32PeekMessageFlags : uint
{
    NoRemove = 0x0000,
    Remove   = 0x0001,
    NoYield  = 0x0002
}
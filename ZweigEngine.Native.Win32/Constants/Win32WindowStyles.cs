namespace ZweigEngine.Native.Win32.Constants;

[Flags]
internal enum Win32WindowStyles
{
	Caption      = 0x00C00000,
	ClipChildren = 0x02000000,
	ClipSiblings = 0x04000000,
	MaximizeBox  = 0x00010000,
	MinimizeBox  = 0x00020000,
	Overlapped   = 0x00000000,
	Popup        = unchecked((int)0x80000000),
	SystemMenu   = 0x00080000,
	ThickFrame   = 0x00040000,
	Border       = 0x00800000,
}
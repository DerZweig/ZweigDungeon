namespace ZweigEngine.Native.DirectX.Imports.DXGI.Constants;

[Flags]
public enum DXGIPresentFlags : uint
{
	None                 = 0,
	Test                 = 0x00000001,
	DoNotSequence        = 0x00000002,
	Restart              = 0x00000004,
	DoNotWait            = 0x00000008,
	StereoPresentRight   = 0x00000010,
	StereoTemporaryMonot = 0x00000020,
	RestrictOutput       = 0x00000040,
	UseDuration          = 0x00000100,
	AllowTearing         = 0x00000200
}
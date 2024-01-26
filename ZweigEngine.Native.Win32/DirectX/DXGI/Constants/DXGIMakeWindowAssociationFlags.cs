namespace ZweigEngine.Native.Win32.DirectX.DXGI.Constants;

[Flags]
public enum DXGIMakeWindowAssociationFlags
{
	NoWindowChanges = 1 << 0,
	NoAltEnter      = 1 << 1,
	NoPrintScreen   = 1 << 2
}
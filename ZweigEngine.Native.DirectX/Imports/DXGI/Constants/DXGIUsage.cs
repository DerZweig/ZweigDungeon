namespace ZweigEngine.Native.DirectX.Imports.DXGI.Constants;

public enum DXGIUsage 
{
	ShaderInput        = 0x00000010,
	RenderTargetOutput = 0x00000020,
	BackBuffer         = 0x00000040,
	Shared             = 0x00000080,
	Readonly           = 0x00000100,
	DiscardOnPresent   = 0x00000200,
	UnorderedAccess    = 0x00000400
}
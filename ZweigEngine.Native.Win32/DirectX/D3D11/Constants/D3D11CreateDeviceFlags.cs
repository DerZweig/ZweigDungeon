namespace ZweigEngine.Native.Win32.DirectX.D3D11.Constants;

[Flags]
public enum D3D11CreateDeviceFlags
{
	Singlethreaded                           = 0x1,
	Debug                                    = 0x2,
	SwitchToRef                              = 0x4,
	PreventInternalThreadingOptimizations    = 0x8,
	BGRASupport                              = 0x20,
	Debuggable                               = 0x40,
	PreventAlteringLayerSettingsFromRegistry = 0x80,
	DisableGPUTimeout                        = 0x100,
	VideoSupport                             = 0x800
}
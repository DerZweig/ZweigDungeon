using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Interfaces;
using ZweigEngine.Native.Win32.Prototypes;

namespace ZweigEngine.Native.Win32;

public class Win32IgnoreDpiScalingConfig : IWin32ProcessDpiScalingConfig
{
	
}

public class Win32UsePerMonitorDpiScalingConfig : IWin32ProcessDpiScalingConfig
{
	public Win32UsePerMonitorDpiScalingConfig(NativeLibraryLoader libraryLoader)
	{
		if (libraryLoader.TryLoadFunction<PfnSetProcessDpiAwarenessDelegate>("Shcore", "SetProcessDpiAwareness", out var func))
		{
			func(Win32ProcessDpiAwareness.PerMonitorDpiAware);
		}
	}
}


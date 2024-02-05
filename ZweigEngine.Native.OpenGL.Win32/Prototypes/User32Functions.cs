namespace ZweigEngine.Native.OpenGL.Win32.Prototypes;


internal delegate IntPtr PfnGetDeviceContextDelegate(IntPtr windowHandle);

internal delegate bool PfnReleaseDeviceContextDelegate(IntPtr windowHandle, IntPtr deviceContext);

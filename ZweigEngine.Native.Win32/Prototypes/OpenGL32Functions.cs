using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.Prototypes;

internal delegate IntPtr PfnCreateContextDelegate(IntPtr deviceContext);

internal delegate bool PfnDeleteContextDelegate(IntPtr renderContext);

internal delegate IntPtr PfnGetCurrentContextDelegate();

internal delegate IntPtr PfnGetProcAddressDelegate([MarshalAs(UnmanagedType.LPStr)] string name);

internal delegate bool PfnMakeCurrentDelegate(IntPtr deviceContext, IntPtr renderContext);

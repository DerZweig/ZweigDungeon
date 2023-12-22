using System.Runtime.InteropServices;

namespace ZweigDungeon.Native.Win32.Prototypes;

internal delegate nint PfnCreateContextDelegate(nint deviceContext);

internal delegate bool PfnDeleteContextDelegate(nint renderContext);

internal delegate nint PfnGetCurrentContextDelegate();

internal delegate nint PfnGetProcAddressDelegate([MarshalAs(UnmanagedType.LPStr)] string name);

internal delegate bool PfnMakeCurrentDelegate(nint deviceContext, nint renderContext);
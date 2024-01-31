using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Prototypes;

internal delegate int PfnChoosePixelFormatDelegate(IntPtr deviceContext, ref Win32PixelFormatDescriptor pixelFormatDescriptor);

internal delegate int PfnGetPixelFormat(IntPtr deviceContext);

internal delegate bool PfnSetPixelFormatDelegate(IntPtr deviceContext, int pixelFormat, ref Win32PixelFormatDescriptor pixelFormatDescriptor);

internal delegate void PfnSwapBuffersDelegate(IntPtr deviceContext);
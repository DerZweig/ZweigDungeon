using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Prototypes;

internal delegate int PfnChoosePixelFormatDelegate(nint deviceContext, ref Win32PixelFormatDescriptor pixelFormatDescriptor);

internal delegate bool PfnSetPixelFormatDelegate(nint deviceContext, int pixelFormat, ref Win32PixelFormatDescriptor pixelFormatDescriptor);

internal delegate void PfnSwapBuffersDelegate(nint deviceContext);
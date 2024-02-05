using ZweigEngine.Native.DirectX.Imports.Structures;

namespace ZweigEngine.Native.DirectX.Imports.Prototypes;

internal delegate Win32HResult PfnCreateDXGIFactory(IntPtr guid, out IntPtr result);

internal delegate Win32HResult PfnD3D11CreateDevice(IntPtr pAdapter, uint driverType,
                                                    IntPtr software, uint flags, IntPtr pFeatureLevels,
                                                    uint featureLevels, uint sdkVersion,
                                                    out IntPtr ppDevice, out uint pFeatureLevel, out IntPtr ppImmediateContext);
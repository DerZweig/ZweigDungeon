using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.Constants;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.Prototypes;

internal delegate bool PfnAdjustWindowRectExDelegate(ref Win32Rectangle rect, Win32WindowStyles styles, bool hasMenu, Win32WindowExtendedStyles extendedStyles);

internal delegate IntPtr PfnBeginPaintDelegate(IntPtr windowHandle, ref Win32PaintStruct paintStruct);

internal delegate IntPtr PfnCreateWindowExDelegate(Win32WindowExtendedStyles extendedStyles,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string className,
                                                   [MarshalAs(UnmanagedType.LPWStr)] string windowTitle,
                                                   Win32WindowStyles windowStyles,
                                                   int left,
                                                   int right,
                                                   int width,
                                                   int height,
                                                   IntPtr parentHandle,
                                                   IntPtr menuHandle,
                                                   IntPtr instanceHandle,
                                                   IntPtr parameter);

internal delegate IntPtr PfnDefaultWindowDelegate(IntPtr window, Win32MessageType message, IntPtr wParam, IntPtr lParam);

internal delegate bool PfnDestroyWindowDelegate(IntPtr windowHandle);

internal delegate IntPtr PfnDispatchMessageDelegate(ref Win32Message message);

internal delegate bool PfnEndPaintDelegate(IntPtr windowHandle, ref Win32PaintStruct paintStruct);

internal delegate bool PfnGetWindowRectDelegate(IntPtr windowHandle, ref Win32Rectangle rectangle);

internal delegate bool PfnGetClientRectDelegate(IntPtr windowHandle, ref Win32Rectangle rectangle);

internal delegate IntPtr PfnGetDeviceContextDelegate(IntPtr windowHandle);

internal delegate short PfnGetKeyStateDelegate(Win32VirtualKey virtualKey);

internal delegate bool PfnGetMessageDelegate(ref Win32Message message, IntPtr windowHandle, uint messageFilterMin, uint messageFilterMax);

internal delegate long PfnGetMessageTimeDelegate();

internal delegate int PfnGetWindowLongDelegate(IntPtr windowHandle, Win32WindowLongIndex index);

internal delegate bool PfnInvalidateRectDelegate(IntPtr windowHandle, IntPtr rectanglePointer, bool eraseBackground);

internal delegate IntPtr PfnLoadCursorDelegate(IntPtr instanceHandle, IntPtr resource);

internal delegate IntPtr PfnLoadIconDelegate(IntPtr instanceHandle, IntPtr resource);

internal delegate uint PfnMapVirtualKeyDelegate(uint code, Win32MapVirtualKeyType mapType);

internal delegate int PfnMessageBoxDelegate(IntPtr windowHandle,
                                            [MarshalAs(UnmanagedType.LPWStr)] string text,
                                            [MarshalAs(UnmanagedType.LPWStr)] string caption,
                                            Win32MessageBoxOptions options);

internal delegate uint PfnMsgWaitForMultipleObjectsDelegate(uint nCount, IntPtr[] handles, bool waitAll, uint milliseconds, Win32QueueStatusFlags wakeMask);

internal delegate bool PfnPeekMessageDelegate(ref Win32Message message, IntPtr windowHandle, uint messageFilterMin, uint messageFilterMax, Win32PeekMessageFlags remove);

internal delegate ushort PfnRegisterClassExDelegate(ref Win32WindowClassEx windowClass);

internal delegate bool PfnReleaseDeviceContextDelegate(IntPtr windowHandle, IntPtr deviceContext);

internal delegate IntPtr PfnSetFocusDelegate(IntPtr windowHandle);

internal delegate bool PfnSetForegroundWindowDelegate(IntPtr windowHandle);

internal delegate int PfnSetWindowLongDelegate(IntPtr windowHandle, Win32WindowLongIndex index, int value);

internal delegate bool PfnSetWindowPosDelegate(IntPtr windowHandle, IntPtr insertAfter, int x, int y, int width, int height, Win32SetWindowPositionCommands commands);

internal delegate bool PfnSetWindowTextDelegate(IntPtr windowHandle, [MarshalAs(UnmanagedType.LPWStr)] string text);

internal delegate bool PfnShowWindowDelegate(IntPtr windowHandle, Win32ShowWindowCommands commands);

internal delegate IntPtr PfnTranslateMessageDelegate(ref Win32Message message);

internal delegate bool PfnUnregisterClassDelegate([MarshalAs(UnmanagedType.LPWStr)] string className, IntPtr instanceHandle);

internal delegate bool PfnGetMonitorInfo(IntPtr monitor, ref Win32MonitorInfo info);

internal delegate bool PfnEnumDisplaySettings( [MarshalAs(UnmanagedType.LPWStr)] string deviceName, Win32DisplaySettingsName name, ref Win32DeviceMode mode);
using System.Runtime.InteropServices;
using ZweigDungeon.Native.Win32.Constants;
using ZweigDungeon.Native.Win32.Structures;

namespace ZweigDungeon.Native.Win32.Prototypes;

internal delegate bool PfnAdjustWindowRectExDelegate(ref Win32Rectangle rect, Win32WindowStyles styles, bool hasMenu, Win32WindowExtendedStyles extendedStyles);

internal delegate nint PfnBeginPaintDelegate(nint windowHandle, ref Win32PaintStruct paintStruct);

internal delegate nint PfnCreateWindowExDelegate(Win32WindowExtendedStyles                extendedStyles,
                                                 [MarshalAs(UnmanagedType.LPWStr)] string className,
                                                 [MarshalAs(UnmanagedType.LPWStr)] string windowTitle,
                                                 Win32WindowStyles                        windowStyles,
                                                 int                                      left,
                                                 int                                      right,
                                                 int                                      width,
                                                 int                                      height,
                                                 nint                                     parentHandle,
                                                 nint                                     menuHandle,
                                                 nint                                     instanceHandle,
                                                 nint                                     parameter);

internal delegate nint PfnDefaultWindowDelegate(nint window, Win32MessageType message, nint wParam, nint lParam);

internal delegate bool PfnDestroyWindowDelegate(nint windowHandle);

internal delegate nint PfnDispatchMessageDelegate(ref Win32Message message);

internal delegate bool PfnEndPaintDelegate(nint windowHandle, ref Win32PaintStruct paintStruct);

internal delegate bool PfnGetWindowRectDelegate(nint windowHandle, ref Win32Rectangle rectangle);

internal delegate bool PfnGetClientRectDelegate(nint windowHandle, ref Win32Rectangle rectangle);

internal delegate nint PfnGetDeviceContextDelegate(nint windowHandle);

internal delegate short PfnGetKeyStateDelegate(Win32VirtualKey virtualKey);

internal delegate bool PfnGetMessageDelegate(ref Win32Message message, nint windowHandle, uint messageFilterMin, uint messageFilterMax);

internal delegate long PfnGetMessageTimeDelegate();

internal delegate int PfnGetWindowLongDelegate(nint windowHandle, Win32WindowLongIndex index);

internal delegate bool PfnInvalidateRectDelegate(nint windowHandle, nint rectanglePointer, bool eraseBackground);

internal delegate nint PfnLoadCursorDelegate(nint instanceHandle, nint resource);

internal delegate nint PfnLoadIconDelegate(nint instanceHandle, nint resource);

internal delegate uint PfnMapVirtualKeyDelegate(uint code, Win32MapVirtualKeyType mapType);

internal delegate int PfnMessageBoxDelegate(nint                                     windowHandle,
                                            [MarshalAs(UnmanagedType.LPWStr)] string text,
                                            [MarshalAs(UnmanagedType.LPWStr)] string caption,
                                            Win32MessageBoxOptions                   options);

internal delegate uint PfnMsgWaitForMultipleObjectsDelegate(uint nCount, nint[] handles, bool waitAll, uint milliseconds, Win32QueueStatusFlags wakeMask);

internal delegate bool PfnPeekMessageDelegate(ref Win32Message message, nint windowHandle, uint messageFilterMin, uint messageFilterMax, Win32PeekMessageFlags remove);

internal delegate ushort PfnRegisterClassExDelegate(ref Win32WindowClassEx windowClass);

internal delegate bool PfnReleaseDeviceContextDelegate(nint windowHandle, nint deviceContext);

internal delegate nint PfnSetFocusDelegate(nint windowHandle);

internal delegate bool PfnSetForegroundWindowDelegate(nint windowHandle);

internal delegate int PfnSetWindowLongDelegate(nint windowHandle, Win32WindowLongIndex index, int value);

internal delegate bool PfnSetWindowPosDelegate(nint windowHandle, nint insertAfter, int x, int y, int width, int height, Win32SetWindowPositionCommands commands);

internal delegate bool PfnSetWindowTextDelegate(nint windowHandle, [MarshalAs(UnmanagedType.LPWStr)] string text);

internal delegate bool PfnShowWindowDelegate(nint windowHandle, Win32ShowWindowCommands commands);

internal delegate nint PfnTranslateMessageDelegate(ref Win32Message message);

internal delegate bool PfnUnregisterClassDelegate([MarshalAs(UnmanagedType.LPWStr)] string className, nint instanceHandle);
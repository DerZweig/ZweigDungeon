using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.Constants;

namespace ZweigEngine.Native.Win32.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct Win32PixelFormatDescriptor
{
    public ushort                               nSize;
    public ushort                               nVersion;
    public Win32PixelFormatDescriptorFlags      dwFlags;
    public Win32PixelFormatDescriptorPixelType  iPixelType;
    public byte                                 cColorBits;
    public byte                                 cRedBits;
    public byte                                 cRedShift;
    public byte                                 cGreenBits;
    public byte                                 cGreenShift;
    public byte                                 cBlueBits;
    public byte                                 cBlueShift;
    public byte                                 cAlphaBits;
    public byte                                 cAlphaShift;
    public byte                                 cAccumBits;
    public byte                                 cAccumRedBits;
    public byte                                 cAccumGreenBits;
    public byte                                 cAccumBlueBits;
    public byte                                 cAccumAlphaBits;
    public byte                                 cDepthBits;
    public byte                                 cStencilBits;
    public byte                                 cAuxBuffers;
    public Win32PixelFormatDescriptorLayerTypes iLayerType;
    public byte                                 bReserved;
    public uint                                 dwLayerMask;
    public uint                                 dwVisibleMask;
    public uint                                 dwDamageMask;
}
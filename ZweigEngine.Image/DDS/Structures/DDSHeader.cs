using System.Runtime.InteropServices;
using ZweigEngine.Image.DDS.Constants;

namespace ZweigEngine.Image.DDS.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct DDSHeader
{
    public readonly uint Magic;

    public readonly uint Size;

    public readonly DDSFlags Flags;

    public readonly uint Height;

    public readonly uint Width;

    public readonly uint PitchOrLinearSize;

    public readonly uint Depth;

    public readonly uint MipMapCount;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
    public readonly uint[] Reserved1;

    public readonly DDSPixelFormat PixelFormat;

    public readonly uint Caps;

    public readonly DDSCaps2 Caps2;

    public readonly uint Caps3;

    public readonly uint Caps4;

    public readonly uint Reserved2;
}
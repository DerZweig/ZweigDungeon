namespace ZweigEngine.Image.DDS.Constants;

[Flags]
internal enum DDSPixelFormatFlags : uint
{
    Alphapixels = 0x1,
    Alpha       = 0x2,
    Fourcc      = 0x4,
    RGB         = 0x40,
    Yuv         = 0x200,
    Luminance   = 0x20000,
}
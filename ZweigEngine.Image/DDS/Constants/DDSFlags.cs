namespace ZweigEngine.Image.DDS.Constants;

[Flags]
internal enum DDSFlags : uint
{
    Caps        = 0x1,
    Height      = 0x2,
    Width       = 0x4,
    Pitch       = 0x8,
    Pixelformat = 0x1000,
    Mipmapcount = 0x20000,
    Linearsize  = 0x80000,
    Volume      = 0x800000
}
using System.Runtime.InteropServices;
using ZweigDungeon.Common.Assets.Images.DDS.Constants;

namespace ZweigDungeon.Common.Assets.Images.DDS.Structures;

[StructLayout(LayoutKind.Sequential), Serializable]
internal struct DDSPixelFormat
{
    public readonly uint Size;

    public readonly DDSPixelFormatFlags Flags;

    public readonly DDSCompressionCode FourCC;

    public readonly uint RGBBitCount;

    public readonly uint RBitMask;

    public readonly uint GBitMask;

    public readonly uint BBitMask;

    public readonly uint ABitMask;
}
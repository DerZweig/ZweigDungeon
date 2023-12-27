using System.Runtime.InteropServices;
using ZweigDungeon.Common.Assets.Images.TGA.Constants;

namespace ZweigDungeon.Common.Assets.Images.TGA.Structures;

[StructLayout(LayoutKind.Sequential)]
internal struct TGAHeader
{
    public byte                    IdLength;
    public byte                    ColourMapLength;
    public TGAImageType            ImageType;
    public short                   ColorMapFirst;
    public short                   ColorMapCount;
    public byte                    BitsPerEntry;
    public short                   OriginLeft;
    public short                   OriginTop;
    public short                   SizeWidth;
    public short                   SizeHeight;
    public byte                    BitsPerPixel;
    public TGAImageDescriptorFlags Descriptor;
}
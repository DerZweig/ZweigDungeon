using System.Runtime.InteropServices;
using ZweigEngine.Image.TGA.Constants;

namespace ZweigEngine.Image.TGA.Structures;

[StructLayout(LayoutKind.Explicit)]
internal struct TGAHeader
{
	[FieldOffset(0)]
	public byte                    IdLength;
	[FieldOffset(1)]
	public byte                    ColorMapType;
	[FieldOffset(2)]
	public TGAImageType            ImageType;
	[FieldOffset(3)]
	public short                   ColorMapOrigin;
	[FieldOffset(5)]
	public short                   ColorMapLength;
	[FieldOffset(7)]
	public byte                    ColorMapDepth;
	[FieldOffset(8)]
	public short                   OriginLeft;
	[FieldOffset(10)]
	public short                   OriginTop;
	[FieldOffset(12)]
	public short                   SizeWidth;
	[FieldOffset(14)]
	public short                   SizeHeight;
	[FieldOffset(16)]
	public byte                    BitsPerPixel;
	[FieldOffset(17)]
	public TGAImageDescriptorFlags Descriptor;
}
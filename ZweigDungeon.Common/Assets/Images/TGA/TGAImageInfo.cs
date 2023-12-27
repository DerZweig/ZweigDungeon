using ZweigDungeon.Common.Assets.Images.TGA.Constants;

namespace ZweigDungeon.Common.Assets.Images.TGA;

internal class TGAImageInfo : IImageInfo
{
	public long                    StreamPosition { get; internal set; }
	public ImagePixelFormat        ImagePixelType { get; internal set; }
	public int                     Height         { get; internal set; }
	public int                     Width          { get; internal set; }
	public TGAImageType            FileType       { get; internal set; }
	public TGAImageDescriptorFlags FileDescriptor { get; internal set; }
}
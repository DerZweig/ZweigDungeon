using ZweigEngine.Common.Assets.Image.Constants;
using ZweigEngine.Common.Assets.Image.Interfaces;
using ZweigEngine.Common.Assets.Image.TGA.Constants;

namespace ZweigEngine.Common.Assets.Image.TGA;

internal class TGAImageInfo : IImageInfo
{
	public long                    StreamPosition { get; internal set; }
	public ImagePixelFormat        PixelType { get; internal set; }
	public int                     Height         { get; internal set; }
	public int                     Width          { get; internal set; }
	public TGAImageType            FileType       { get; internal set; }
	public TGAImageDescriptorFlags FileDescriptor { get; internal set; }
}
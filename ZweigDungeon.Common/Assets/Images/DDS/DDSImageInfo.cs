using ZweigDungeon.Common.Assets.Images.DDS.Constants;
using ZweigDungeon.Common.Assets.Images.DDS.Decoder;

namespace ZweigDungeon.Common.Assets.Images.DDS;

internal sealed class DDSImageInfo : IImageInfo
{
	public long             StreamPosition { get; internal set; }
	public ImagePixelFormat ImagePixelType { get; internal set; }
	public int              Height         { get; internal set; }
	public int              Width          { get; internal set; }
	public DDSImageFormat   FileFormat     { get; internal set; }
	public DDSDecoder       FileDecoder    { get; internal set; } = null!;
}
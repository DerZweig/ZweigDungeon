using ZweigEngine.Common.Assets.Image.Constants;
using ZweigEngine.Common.Assets.Image.DDS.Constants;
using ZweigEngine.Common.Assets.Image.DDS.Decoder;
using ZweigEngine.Common.Assets.Image.Interfaces;

namespace ZweigEngine.Common.Assets.Image.DDS;

internal sealed class DDSImageInfo : IImageInfo
{
	public long             StreamPosition { get; internal set; }
	public ImagePixelFormat PixelType      { get; internal set; }
	public int              Height         { get; internal set; }
	public int              Width          { get; internal set; }
	public DDSImageFormat   FileFormat     { get; internal set; }
	public DDSDecoder       FileDecoder    { get; internal set; } = null!;
}
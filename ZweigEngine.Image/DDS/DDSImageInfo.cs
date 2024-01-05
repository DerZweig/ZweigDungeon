using ZweigEngine.Image.DDS.Constants;
using ZweigEngine.Image.DDS.Decoder;

namespace ZweigEngine.Image.DDS;

internal sealed class DDSImageInfo : IImageInfo
{
	public long             StreamPosition { get; internal set; }
	public ImagePixelFormat PixelType { get; internal set; }
	public int              Height         { get; internal set; }
	public int              Width          { get; internal set; }
	public DDSImageFormat   FileFormat     { get; internal set; }
	public DDSDecoder       FileDecoder    { get; internal set; } = null!;
}
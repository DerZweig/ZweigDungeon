using ZweigEngine.Common.Assets.Image.Constants;

namespace ZweigEngine.Common.Assets.Image.Interfaces;

public interface IImageInfo
{
	ImagePixelFormat PixelType { get; }
	int              Width          { get; }
	int              Height         { get; }
}

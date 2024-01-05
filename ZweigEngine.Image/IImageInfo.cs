namespace ZweigEngine.Image;

public interface IImageInfo
{
	ImagePixelFormat PixelType { get; }
	int              Width          { get; }
	int              Height         { get; }
}

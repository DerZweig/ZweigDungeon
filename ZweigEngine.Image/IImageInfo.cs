namespace ZweigEngine.Image;

public interface IImageInfo
{
	ImagePixelFormat ImagePixelType { get; }
	int              Width          { get; }
	int              Height         { get; }
}

namespace ZweigDungeon.Common.Assets.Images;

public interface IImageInfo
{
	ImagePixelFormat ImagePixelType { get; }
	int              Width          { get; }
	int              Height         { get; }
}

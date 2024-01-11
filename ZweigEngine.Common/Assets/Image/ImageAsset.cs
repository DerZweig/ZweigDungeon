using ZweigEngine.Common.Assets.Image.Constants;

namespace ZweigEngine.Common.Assets.Image;

public sealed class ImageAsset
{
	static ImageAsset()
	{
		Empty = new ImageAsset();
	}

	public static ImageAsset Empty { get; }

	public int                 Width  { get; init; } = 0;
	public int                 Height { get; init; } = 0;
	public ImagePixelFormat    Format { get; set; }  = ImagePixelFormat.Unknown;
	public IReadOnlyList<byte> Data   { get; init; } = Array.Empty<byte>();
}
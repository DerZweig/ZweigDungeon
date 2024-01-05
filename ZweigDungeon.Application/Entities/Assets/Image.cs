using ZweigEngine.Image;

namespace ZweigDungeon.Application.Entities.Assets;

public class Image : IImageInfo
{
	public ImagePixelFormat    PixelType { get; init; }
	public int                 Width     { get; init; }
	public int                 Height    { get; init; }
	public IReadOnlyList<byte> PixelData { get; init; } = null!;
}
namespace ZweigEngine.Common.Assets.Image.Interfaces;

public interface IImageReader
{
	Task<IImageInfo> LoadInfoBlockAsync(Stream stream, CancellationToken cancellationToken);

	Task<IReadOnlyList<byte>> LoadPixelDataAsync(Stream stream, IImageInfo imageInfo, CancellationToken cancellationToken);
}
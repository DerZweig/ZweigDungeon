using ZweigEngine.Common.Assets.Image;
using ZweigEngine.Common.Assets.Image.DDS;
using ZweigEngine.Common.Assets.Image.Interfaces;
using ZweigEngine.Common.Assets.Image.TGA;
using ZweigEngine.Common.Services.Interfaces.Files;

namespace ZweigEngine.Common.Services.Repositories;

public class ImageRepository : BasicAsyncRepository<ImageAsset>
{
	private readonly IFileSystem                      m_config;
	private readonly Dictionary<string, IImageReader> m_readers;

	public ImageRepository(IFileSystem config)
	{
		m_config = config;
		m_readers = new Dictionary<string, IImageReader>
		{
			{ ".dds", new DDSImageReader() },
			{ ".tga", new TGAImageReader() }
		};
	}

	public Task RegisterImageReader(string extension, IImageReader reader) => Synchronize(() =>
	{
		m_readers.Add(extension, reader);
	}, CancellationToken.None);

	protected override async Task<ImageAsset> LoadContents(string path, CancellationToken cancellationToken)
	{
		var dataPath = m_config.GetDataPath(path);
		var info     = (IImageInfo?)null;
		var data     = (IReadOnlyList<byte>?)null;

		foreach (var (extension, reader) in m_readers)
		{
			var extPath = Path.ChangeExtension(dataPath, extension);

			if (m_config.FileExists(extPath))
			{
				await using (var stream = m_config.OpenRead(extPath))
				{
					info = await reader.LoadInfoBlockAsync(stream, cancellationToken).ConfigureAwait(false);
					data = await reader.LoadPixelDataAsync(stream, info, cancellationToken).ConfigureAwait(false);
				}

				break;
			}
		}

		if (info == null || data == null)
		{
			throw new FileLoadException($"Couldn't load image {path}");
		}

		return new ImageAsset
		{
			Width  = info.Width,
			Height = info.Height,
			Format = info.PixelType,
			Data   = data
		};
	}
}
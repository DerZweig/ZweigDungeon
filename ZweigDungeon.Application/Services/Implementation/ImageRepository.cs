using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Utility.Concurrency;
using ZweigEngine.Image;
using ZweigEngine.Image.DDS;
using ZweigEngine.Image.TGA;

namespace ZweigDungeon.Application.Services.Implementation;

public class ImageRepository : IImageRepository
{
	private readonly IGlobalCancellation       m_cancellation;
	private readonly ExclusiveTaskFactory      m_synchronization;
	private readonly IImageReader              m_ddsImageReader;
	private readonly IImageReader              m_tgaImageReader;
	private readonly Dictionary<string, Entry> m_images;

	public ImageRepository(IGlobalCancellation cancellation)
	{
		m_cancellation    = cancellation;
		m_synchronization = new ExclusiveTaskFactory();
		m_ddsImageReader  = new DDSImageReader();
		m_tgaImageReader  = new TGAImageReader();
		m_images          = new Dictionary<string, Entry>();
	}

	public Task<IImageInfo> LoadImage(string name) => m_synchronization.Invoke<IImageInfo>(async () =>
	{
		var normalized = name.Trim().ToLower();
		if (m_images.TryGetValue(normalized, out var entry))
		{
			if (entry.Pending != null)
			{
				await entry.Pending;
			}

			return entry.Image ?? throw new NullReferenceException();
		}

		entry = new Entry();
		m_images.Add(normalized, entry);

		var worker = LoadImageFile(name);
		entry.Pending = worker;
		entry.Image   = await worker;
		entry.Pending = null;
		return entry.Image ?? throw new NullReferenceException();
	}, m_cancellation.Token);

	private async Task<Image> LoadImageFile(string name)
	{
		var path              = Path.Combine("Data", name.Trim());
		var ddsPath           = Path.ChangeExtension(path, ".dds");
		var tgaPath           = Path.ChangeExtension(path, ".tga");
		var info              = (IImageInfo?)null;
		var data              = (IReadOnlyList<byte>?)null;
		var cancellationToken = m_cancellation.Token;

		if (File.Exists(ddsPath))
		{
			await using (var stream = File.OpenRead(ddsPath))
			{
				info = await m_ddsImageReader.LoadInfoBlockAsync(stream, cancellationToken).ConfigureAwait(false);
				data = await m_ddsImageReader.LoadPixelDataAsync(stream, info, cancellationToken).ConfigureAwait(false);
			}
		}
		else if (File.Exists(tgaPath))
		{
			await using (var stream = File.OpenRead(tgaPath))
			{
				info = await m_tgaImageReader.LoadInfoBlockAsync(stream, cancellationToken).ConfigureAwait(false);
				data = await m_tgaImageReader.LoadPixelDataAsync(stream, info, cancellationToken).ConfigureAwait(false);
			}
		}

		if (info == null || data == null)
		{
			throw new FileLoadException($"Couldn't load image {name}");
		}

		return new Image
		{
			PixelType = info.PixelType,
			Width     = info.Width,
			Height    = info.Height,
			PixelData = data
		};
	}

	private class Entry
	{
		public Task?  Pending { get; set; }
		public Image? Image   { get; set; }
	}
}
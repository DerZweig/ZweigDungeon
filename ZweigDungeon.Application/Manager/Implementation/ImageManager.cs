using ZweigDungeon.Application.Manager.Interfaces;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Utility.Concurrency;
using ZweigEngine.Image;
using ZweigEngine.Image.DDS;
using ZweigEngine.Image.TGA;

namespace ZweigDungeon.Application.Manager.Implementation;

public class ImageManager : IDisposable, IImageManager
{
	private readonly IVideoContext                    m_video;
	private readonly ExclusiveTaskFactory             m_sync;
	private readonly DDSImageReader                   m_ddsImageReader;
	private readonly TGAImageReader                   m_tgaImageReader;
	private readonly IVideoImage                      m_undefined;
	private readonly Dictionary<string, IVideoImage?> m_images;

	public ImageManager(IVideoContext video)
	{
		m_video          = video;
		m_sync           = new ExclusiveTaskFactory();
		m_ddsImageReader = new DDSImageReader();
		m_tgaImageReader = new TGAImageReader();
		m_images         = new Dictionary<string, IVideoImage?>();

		m_video.CreateSurface(4, 4, out m_undefined);
		m_undefined.Map(pixels =>
		{
			pixels[0] = new VideoColor { Red = 255, Green = 0, Blue   = 0, Alpha   = 255 };
			pixels[1] = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
			pixels[2] = new VideoColor { Red = 255, Green = 0, Blue   = 0, Alpha   = 255 };
			pixels[3] = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
		});
	}

	private void ReleaseUnmanagedResources()
	{
		var images = m_images.Values.ToArray();
		m_images.Clear();

		foreach (var image in images.Where(x => x != m_undefined))
		{
			image?.Dispose();
		}

		m_undefined.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~ImageManager()
	{
		ReleaseUnmanagedResources();
	}

	public async void Load(string name)
	{
		if (!m_images.TryAdd(name, null))
		{
			return;
		}

		var path    = Path.Combine("Data", name);
		var ddsPath = Path.ChangeExtension(path, ".dds");
		var tgaPath = Path.ChangeExtension(path, ".tga");
		var info    = (IImageInfo?)null;
		var data    = (IReadOnlyList<byte>?)null;

		try
		{
			if (File.Exists(ddsPath))
			{
				await using (var stream = File.OpenRead(ddsPath))
				{
					info = await m_ddsImageReader.LoadInfoBlockAsync(stream, CancellationToken.None);
					data = await m_ddsImageReader.LoadPixelDataAsync(stream, info, CancellationToken.None);
				}
			}
			else if (File.Exists(tgaPath))
			{
				await using (var stream = File.OpenRead(tgaPath))
				{
					info = await m_tgaImageReader.LoadInfoBlockAsync(stream, CancellationToken.None);
					data = await m_tgaImageReader.LoadPixelDataAsync(stream, info, CancellationToken.None);
				}
			}

			if (info == null || data == null)
			{
				return;
			}

			if (info.ImagePixelType is ImagePixelFormat.RGB8 or ImagePixelFormat.RGBA8)
			{
				m_video.CreateSurface((ushort)info.Width, (ushort)info.Height, out var texture);
				m_images[name] = texture;

				texture.Map(pixels =>
				{
					if (info.ImagePixelType == ImagePixelFormat.RGB8)
					{
						for (var index = 0; index < pixels.Length; ++index)
						{
							pixels[index].Red   = data![index * 3 + 0];
							pixels[index].Green = data[index * 3 + 1];
							pixels[index].Blue  = data[index * 3 + 2];
							pixels[index].Alpha = 255;
						}
					}
					else if (info.ImagePixelType == ImagePixelFormat.RGBA8)
					{
						for (var index = 0; index < pixels.Length; ++index)
						{
							pixels[index].Red   = data![index * 4 + 0];
							pixels[index].Green = data[index * 4 + 1];
							pixels[index].Blue  = data[index * 4 + 2];
							pixels[index].Alpha = data[index * 4 + 3];
						}
					}
				});
			}
		}
		finally
		{
			if (data == null)
			{
				m_images[name] = m_undefined;
			}
		}
	}

	public void Bind(string name, Action<IVideoImage> work)
	{
		if (m_images.TryGetValue(name, out var image) && image != null)
		{
			work(image);
		}
	}
}
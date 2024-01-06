using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Image;

namespace ZweigDungeon.Application.Services.Implementation;

public class TextureManager : IDisposable, ITextureManager
{
	private readonly IVideoContext                   m_video;
	private readonly IPlatformSynchronization        m_synchronization;
	private readonly IDictionary<Image, IVideoImage> m_textures;

	public TextureManager(IVideoContext video, IPlatformSynchronization synchronization)
	{
		m_video                = video;
		m_synchronization = synchronization;
		m_textures             = new Dictionary<Image, IVideoImage>();
	}

	private void ReleaseUnmanagedResources()
	{
		Clear();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~TextureManager()
	{
		ReleaseUnmanagedResources();
	}

	public void Clear()
	{
		var textures = m_textures.Values.ToArray();
		m_textures.Clear();
		foreach (var texture in textures)
		{
			texture.Dispose();
		}
	}

	public void Upload(Image image) => m_synchronization.ExecuteWithGuard(() =>
	{
		if (m_textures.TryGetValue(image, out var texture))
		{
			if (image.Width != texture.Width || image.Height != texture.Height)
			{
				throw new Exception("Image dimensions have been changed.");
			}

			switch (image.PixelType)
			{
				case ImagePixelFormat.RGB8:
					UploadRGB8(image, texture);
					break;
				case ImagePixelFormat.RGBA8:
					UploadRGBA8(image, texture);
					break;
				case ImagePixelFormat.R8:
					UploadR8(image, texture);
					break;
				default:
					throw new NotSupportedException("Image pixel format must be RGB8, RGBA8 or R8.");
			}
		}
		else
		{
			if (image.Width >= ushort.MaxValue || image.Height >= ushort.MaxValue || image.Width == 0u || image.Height == 0u)
			{
				throw new NotSupportedException("Invalid image dimensions.");
			}

			switch (image.PixelType)
			{
				case ImagePixelFormat.RGB8:
					m_video.CreateSurface((ushort)image.Width, (ushort)image.Height, out texture);
					m_textures[image] = texture;
					UploadRGB8(image, texture);
					break;
				case ImagePixelFormat.RGBA8:
					m_video.CreateSurface((ushort)image.Width, (ushort)image.Height, out texture);
					m_textures[image] = texture;
					UploadRGBA8(image, texture);
					break;
				case ImagePixelFormat.R8:
					m_video.CreateSurface((ushort)image.Width, (ushort)image.Height, out texture);
					m_textures[image] = texture;
					UploadR8(image, texture);
					break;
				default:
					throw new NotSupportedException("Image pixel format must be RGB8, RGBA8 or R8.");
			}
		}
	});

	public void Bind(Image image, Action<IVideoImage> work) => m_synchronization.ExecuteWithGuard(() =>
	{
		if (m_textures.TryGetValue(image, out var texture))
		{
			work(texture);
		}
	});

	private static void UploadR8(Image image, IVideoImage texture)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.PixelData[index];
				dst.Green = image.PixelData[index];
				dst.Blue  = image.PixelData[index];
				dst.Alpha = image.PixelData[index];
			}
		});
	}

	private static void UploadRGB8(Image image, IVideoImage texture)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.PixelData[index * 3 + 0];
				dst.Green = image.PixelData[index * 3 + 1];
				dst.Blue  = image.PixelData[index * 3 + 2];
				dst.Alpha = 255;
			}
		});
	}

	private static void UploadRGBA8(Image image, IVideoImage texture)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.PixelData[index * 4 + 0];
				dst.Green = image.PixelData[index * 4 + 1];
				dst.Blue  = image.PixelData[index * 4 + 2];
				dst.Alpha = image.PixelData[index * 4 + 3];
			}
		});
	}
}
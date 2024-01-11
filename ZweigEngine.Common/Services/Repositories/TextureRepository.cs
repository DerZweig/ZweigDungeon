using ZweigEngine.Common.Assets.Image;
using ZweigEngine.Common.Assets.Image.Constants;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigEngine.Common.Services.Repositories;

public class TextureRepository : BasicVideoRepository<ImageAsset, IVideoImage>
{
	private readonly IVideoContext m_video;

	public TextureRepository(IPlatformSynchronization synchronization, IVideoContext video) : base(synchronization)
	{
		m_video = video;
	}

	protected override IVideoImage CreateResource(ImageAsset asset)
	{
		if (asset.Width >= ushort.MaxValue || asset.Height >= ushort.MaxValue || asset.Width == 0u || asset.Height == 0u)
		{
			throw new NotSupportedException("Invalid image dimensions.");
		}

		switch (asset.Format)
		{
			case ImagePixelFormat.RGB8:
			case ImagePixelFormat.RGBA8:
			case ImagePixelFormat.R8:
				m_video.CreateSurface((ushort)asset.Width, (ushort)asset.Height, out var texture);
				return texture;
			default:
				throw new NotSupportedException("Image pixel format must be RGB8, RGBA8 or R8.");
		}
	}

	protected override void UploadResource(IVideoImage resource, ImageAsset asset)
	{
		if (asset.Width != resource.Width || asset.Height != resource.Height)
		{
			throw new Exception("Image dimensions have been changed.");
		}

		switch (asset.Format)
		{
			case ImagePixelFormat.RGB8:
				UploadRGB8(resource, asset);
				break;
			case ImagePixelFormat.RGBA8:
				UploadRGBA8(resource, asset);
				break;
			case ImagePixelFormat.R8:
				UploadR8(resource, asset);
				break;
			default:
				throw new NotSupportedException("Image pixel format must be RGB8, RGBA8 or R8.");
		}
	}

	private static void UploadR8(IVideoImage texture, ImageAsset image)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.Data[index];
				dst.Green = image.Data[index];
				dst.Blue  = image.Data[index];
				dst.Alpha = image.Data[index];
			}
		});
	}

	private static void UploadRGB8(IVideoImage texture, ImageAsset image)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.Data[index * 3 + 0];
				dst.Green = image.Data[index * 3 + 1];
				dst.Blue  = image.Data[index * 3 + 2];
				dst.Alpha = 255;
			}
		});
	}

	private static void UploadRGBA8(IVideoImage texture, ImageAsset image)
	{
		texture.Map(pixels =>
		{
			for (var index = 0; index < pixels.Length; ++index)
			{
				ref var dst = ref pixels[index];
				dst.Red   = image.Data[index * 4 + 0];
				dst.Green = image.Data[index * 4 + 1];
				dst.Blue  = image.Data[index * 4 + 2];
				dst.Alpha = image.Data[index * 4 + 3];
			}
		});
	}
}
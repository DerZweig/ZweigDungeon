using ZweigDungeon.Common.Assets.Images.TGA.Constants;
using ZweigDungeon.Common.Assets.Images.TGA.Structures;
using ZweigDungeon.Common.Utility;

namespace ZweigDungeon.Common.Assets.Images.TGA;

public sealed class TGAImageReader : IImageReader
{
	public async Task<IImageInfo> LoadInfoBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		var header = await stream.ReadStructureAsync<TGAHeader>(cancellationToken).ConfigureAwait(false);
		if (header.ImageType != TGAImageType.RunLengthEncoded && header.ImageType != TGAImageType.Uncompressed)
		{
			throw new FileLoadException("Unsupported image type.");
		}

		switch (header.BitsPerPixel)
		{
			case 24:
				return new TGAImageInfo
				{
					StreamPosition = stream.Position,
					ImagePixelType = ImagePixelFormat.RGB8,
					Width          = header.SizeWidth,
					Height         = header.SizeHeight,
					FileType       = header.ImageType,
					FileDescriptor = header.Descriptor,
				};
			case 32:
				return new TGAImageInfo
				{
					StreamPosition = stream.Position,
					ImagePixelType = ImagePixelFormat.RGBA8,
					Width          = header.SizeWidth,
					Height         = header.SizeHeight,
					FileType       = header.ImageType,
					FileDescriptor = header.Descriptor,
				};
			default:
				throw new FileLoadException("Unsupported image format.");
		}
	}

	public async Task<IReadOnlyList<byte>> LoadPixelDataAsync(Stream stream, IImageInfo imageInfo, CancellationToken cancellationToken)
	{
		var tgaImage = imageInfo as TGAImageInfo ?? throw new ArgumentOutOfRangeException(nameof(imageInfo));
		var pixelSize = tgaImage.ImagePixelType switch
		                {
			                ImagePixelFormat.RGBA8 => 32,
			                ImagePixelFormat.RGB8 => 24,
			                _ => throw new FileLoadException("Invalid pixel type.")
		                };

		var pixelData = tgaImage.FileType switch
		                {
			                TGAImageType.Uncompressed => await GetUncompressedPixelData(stream, tgaImage, pixelSize, cancellationToken).ConfigureAwait(false),
			                TGAImageType.RunLengthEncoded => await GetRunLengthCompressedPixelData(stream, tgaImage, pixelSize, cancellationToken).ConfigureAwait(false),
			                _ => throw new FileLoadException("Invalid image type.")
		                };

		var decoded = await Task.Run(() => tgaImage.ImagePixelType switch
		                                   {
			                                   ImagePixelFormat.RGB8 => DecodePixelDataRGB(tgaImage, pixelData),
			                                   ImagePixelFormat.RGBA8 => DecodePixelDataRGBA(tgaImage, pixelData),
			                                   _ => throw new FileLoadException("Invalid image color format.")
		                                   }, cancellationToken);

		cancellationToken.ThrowIfCancellationRequested();
		return decoded;
	}

	private static byte[] DecodePixelDataRGB(TGAImageInfo imageInfo, IReadOnlyList<byte> pixelData)
	{
		var writer = new MemoryStream(imageInfo.Width * imageInfo.Height * 3);

		if ((imageInfo.FileDescriptor & TGAImageDescriptorFlags.OriginLower) != 0)
		{
			for (var y = 0; y < imageInfo.Height; ++y)
			{
				var line = y * imageInfo.Width;
				for (var x = 0; x < imageInfo.Width; ++x)
				{
					var srcOffset = (line + x) * 3;
					writer.WriteByte(pixelData[srcOffset + 2]);
					writer.WriteByte(pixelData[srcOffset + 1]);
					writer.WriteByte(pixelData[srcOffset + 0]);
				}
			}
		}
		else
		{
			for (var y = 1; y <= imageInfo.Height; ++y)
			{
				var line = (imageInfo.Height - y) * imageInfo.Width;
				for (var x = 0; x < imageInfo.Width; ++x)
				{
					var srcOffset = (line + x) * 3;
					writer.WriteByte(pixelData[srcOffset + 2]);
					writer.WriteByte(pixelData[srcOffset + 1]);
					writer.WriteByte(pixelData[srcOffset + 0]);
				}
			}
		}

		return writer.ToArray();
	}

	private static byte[] DecodePixelDataRGBA(TGAImageInfo imageInfo, IReadOnlyList<byte> pixelData)
	{
		var writer = new MemoryStream(imageInfo.Width * imageInfo.Height * 4);

		if ((imageInfo.FileDescriptor & TGAImageDescriptorFlags.OriginLower) != 0)
		{
			for (var y = 0; y < imageInfo.Height; ++y)
			{
				var line = y * imageInfo.Width;
				for (var x = 0; x < imageInfo.Width; ++x)
				{
					var srcOffset = (line + x) * 4;
					writer.WriteByte(pixelData[srcOffset + 2]);
					writer.WriteByte(pixelData[srcOffset + 1]);
					writer.WriteByte(pixelData[srcOffset + 0]);
					writer.WriteByte(pixelData[srcOffset + 3]);
				}
			}
		}
		else
		{
			for (var y = 1; y <= imageInfo.Height; ++y)
			{
				var line = (imageInfo.Height - y) * imageInfo.Width;
				for (var x = 0; x < imageInfo.Width; ++x)
				{
					var srcOffset = (line + x) * 4;
					writer.WriteByte(pixelData[srcOffset + 2]);
					writer.WriteByte(pixelData[srcOffset + 1]);
					writer.WriteByte(pixelData[srcOffset + 0]);
					writer.WriteByte(pixelData[srcOffset + 3]);
				}
			}
		}

		return writer.ToArray();
	}

	private static async Task<byte[]> GetRunLengthCompressedPixelData(Stream stream, TGAImageInfo im, int pixelSize, CancellationToken cancellationToken)
	{
		var bpp        = pixelSize / 8;
		var rleData    = new byte[128 * bpp];
		var resultData = new byte[im.Width * im.Height * bpp];
		var resultSize = 0;

		while (resultSize < resultData.Length)
		{
			var rle   = stream.ReadByte();
			var count = (rle & 127) + 1;

			if ((rle & 128u) != 0)
			{
				if (await stream.ReadAsync(rleData.AsMemory(0, bpp), cancellationToken).ConfigureAwait(false) != bpp)
				{
					throw new FileLoadException("Couldn't read image data.");
				}

				for (var i = 0; i < count; ++i)
				{
					Array.Copy(rleData, 0, resultData, resultSize, bpp);
					resultSize += bpp;
				}
			}
			else
			{
				count *= bpp;
				if (await stream.ReadAsync(rleData, 0, count, cancellationToken).ConfigureAwait(false) != count)
				{
					throw new FileLoadException("Couldn't read image data.");
				}

				Array.Copy(rleData, 0, resultData, resultSize, count);
				resultSize += count;
			}
		}

		return resultData;
	}

	private static async Task<byte[]> GetUncompressedPixelData(Stream stream, TGAImageInfo imageInfo, int pixelSize, CancellationToken cancellationToken)
	{
		stream.Seek(imageInfo.StreamPosition, SeekOrigin.Begin);

		var buffer = new byte[imageInfo.Width * imageInfo.Height * pixelSize / 8];
		if (await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false) != buffer.Length)
		{
			throw new FileLoadException("Couldn't read image data.");
		}

		return buffer;
	}
}
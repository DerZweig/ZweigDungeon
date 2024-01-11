using ZweigEngine.Common.Assets.Image.Constants;
using ZweigEngine.Common.Assets.Image.DDS.Constants;
using ZweigEngine.Common.Assets.Image.DDS.Decoder;
using ZweigEngine.Common.Assets.Image.DDS.Structures;
using ZweigEngine.Common.Assets.Image.Interfaces;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Common.Assets.Image.DDS;

internal sealed class DDSImageReader : IImageReader
{
	private const DDSFlags REQUIRED_FLAGS = DDSFlags.Caps | DDSFlags.Width | DDSFlags.Height | DDSFlags.Pixelformat;

	public async Task<IImageInfo> LoadInfoBlockAsync(Stream stream, CancellationToken cancellationToken)
	{
		var header      = await stream.ReadStructureAsync<DDSHeader>(cancellationToken).ConfigureAwait(false);
		var magic       = BitConverter.GetBytes(header.Magic);
		var magicString = System.Text.Encoding.ASCII.GetString(magic);

		if (!string.Equals(magicString, "DDS ", StringComparison.Ordinal))
		{
			throw new FileLoadException("Invalid DDSHeader size.");
		}

		if (header.PixelFormat.Size != 32)
		{
			throw new FileLoadException("Invalid DDSPixelFormat size.");
		}

		if ((header.Flags & REQUIRED_FLAGS) != REQUIRED_FLAGS)
		{
			throw new FileLoadException("Invalid texture flags.");
		}

		if (header.Width == 0u || header.Height == 0u)
		{
			throw new FileLoadException("Invalid texture size.");
		}

		if ((header.Flags & DDSFlags.Volume) != 0 || (header.Caps2 & DDSCaps2.Cubemap) != 0)
		{
			throw new FileLoadException("Invalid texture type.");
		}

		var image = new DDSImageInfo
		{
			FileFormat     = GetImageFormat(header.PixelFormat),
			StreamPosition = stream.Position,
			Width          = (int)header.Width,
			Height         = (int)header.Height
		};

		switch (image.FileFormat)
		{
			case DDSImageFormat.DXT1:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSCompressedDXT1Decoder();
				break;
			case DDSImageFormat.DXT2:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSCompressedDXT2Decoder();
				break;
			case DDSImageFormat.DXT3:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSCompressedDXT3Decoder();
				break;
			case DDSImageFormat.DXT4:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSCompressedDXT4Decoder();
				break;
			case DDSImageFormat.DXT5:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSCompressedDXT5Decoder();
				break;
			case DDSImageFormat.LuminanceHigh:
				image.PixelType   = ImagePixelFormat.R16;
				image.FileDecoder = new DDSUncompressedLuminanceDecoder();
				break;
			case DDSImageFormat.Luminance:
				image.PixelType   = ImagePixelFormat.R8;
				image.FileDecoder = new DDSUncompressedLuminanceDecoder();
				break;
			case DDSImageFormat.ARGB:
			case DDSImageFormat.ABGR:
				image.PixelType   = ImagePixelFormat.RGBA8;
				image.FileDecoder = new DDSUncompressedRGBADecoder();
				break;
			default:
				image.PixelType = ImagePixelFormat.Unknown;
				break;
		}

		image.FileDecoder.InitializeFromHeader(header);
		return image;
	}

	public async Task<IReadOnlyList<byte>> LoadPixelDataAsync(Stream stream, IImageInfo imageInfo, CancellationToken cancellationToken)
	{
		var ddsImage   = imageInfo as DDSImageInfo ?? throw new ArgumentOutOfRangeException(nameof(imageInfo));
		var dataBuffer = await GetUnprocessedFileDataAsync(stream, ddsImage, cancellationToken).ConfigureAwait(false);
		var width      = (uint)imageInfo.Width;
		var height     = (uint)imageInfo.Height;
		var decoded = await Task.Run(() => ddsImage.FileFormat switch
		                                   {
			                                   DDSImageFormat.DXT1 => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.DXT2 => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.DXT3 => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.DXT4 => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.DXT5 => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.Luminance => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.LuminanceHigh => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.ARGB => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   DDSImageFormat.ABGR => ddsImage.FileDecoder.DecodeData(dataBuffer, width, height),
			                                   _ => throw new ArgumentOutOfRangeException(nameof(imageInfo))
		                                   }, cancellationToken);

		cancellationToken.ThrowIfCancellationRequested();
		return decoded;
	}

	private static async Task<byte[]> GetUnprocessedFileDataAsync(Stream stream, DDSImageInfo imageInfo, CancellationToken cancellationToken)
	{
		var dataOffset = CalculateDataSize(imageInfo, 0u);
		var dataSize   = CalculateDataSize(imageInfo, 1u) - dataOffset;
		var dataBuffer = new byte[dataSize];

		stream.Seek(imageInfo.StreamPosition + dataOffset, SeekOrigin.Begin);
		var read = await stream.ReadAsync(dataBuffer, cancellationToken).ConfigureAwait(false);
		if (read != dataBuffer.Length)
		{
			throw new FileLoadException("Failed to load required mip level bytes.");
		}

		return dataBuffer;
	}

	private static long CalculateDataSize(DDSImageInfo imageInfo, in uint mipCount)
	{
		var result = 0ul;
		var width  = (uint)imageInfo.Width;
		var height = (uint)imageInfo.Height;

		for (var iteration = 0u; iteration < mipCount; ++iteration, width >>= 1, height >>= 1)
		{
			result += imageInfo.FileDecoder.GetDataSize(width, height);
		}

		return (long)result;
	}

	private static DDSImageFormat GetImageFormat(in DDSPixelFormat pixelFormat)
	{
		if ((pixelFormat.Flags & DDSPixelFormatFlags.RGB) != 0)
		{
			if (pixelFormat.RGBBitCount != 32)
			{
				return DDSImageFormat.Unsupported;
			}

			if (IsColorBitMask(pixelFormat, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000))
			{
				return DDSImageFormat.ARGB;
			}

			if (IsColorBitMask(pixelFormat, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000))
			{
				return DDSImageFormat.ABGR;
			}
		}
		else if ((pixelFormat.Flags & DDSPixelFormatFlags.Luminance) != 0)
		{
			if (pixelFormat.RGBBitCount == 8)
			{
				if (IsColorBitMask(pixelFormat, 0x000000ff, 0x00000000, 0x00000000, 0x00000000))
				{
					return DDSImageFormat.Luminance;
				}
			}

			if (pixelFormat.RGBBitCount != 16)
			{
				return DDSImageFormat.Unsupported;
			}

			if (IsColorBitMask(pixelFormat, 0x0000ffff, 0x00000000, 0x00000000, 0x00000000))
			{
				return DDSImageFormat.LuminanceHigh;
			}
		}
		else if ((pixelFormat.Flags & DDSPixelFormatFlags.Fourcc) != 0)
		{
			return pixelFormat.FourCC switch
			       {
				       DDSCompressionCode.DXT1 => DDSImageFormat.DXT1,
				       DDSCompressionCode.DXT2 => DDSImageFormat.DXT2,
				       DDSCompressionCode.DXT3 => DDSImageFormat.DXT3,
				       DDSCompressionCode.DXT4 => DDSImageFormat.DXT4,
				       DDSCompressionCode.DXT5 => DDSImageFormat.DXT5,
				       _ => DDSImageFormat.Unsupported
			       };
		}

		return DDSImageFormat.Unsupported;
	}

	private static bool IsColorBitMask(in DDSPixelFormat pixelFormat, in uint r, in uint g, in uint b, in uint a)
	{
		return pixelFormat.RBitMask == r &&
		       pixelFormat.GBitMask == g &&
		       pixelFormat.BBitMask == b &&
		       pixelFormat.ABitMask == a;
	}
}
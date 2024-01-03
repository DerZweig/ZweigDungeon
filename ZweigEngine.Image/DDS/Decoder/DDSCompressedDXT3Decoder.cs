using ZweigEngine.Image.DDS.Structures;

namespace ZweigEngine.Image.DDS.Decoder;

internal class DDSCompressedDXT3Decoder : DDSCompressedDecoder
{
    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        var       block  = new byte[16];
        var       colors = new DDSColour8888[4];
        var       result = new byte[width * height * 4];
        var       stride = width * 4;
        using var stream = new MemoryStream(input);
        using var reader = new BinaryReader(stream);

        for (var iterHeight = 0u; iterHeight < height; iterHeight += 4)
        {
            for (var iterWidth = 0u; iterWidth < width; iterWidth += 4)
            {
                var alphas  = reader.ReadBytes(8);
                var coldat  = reader.ReadBytes(4);
                var bitmask = reader.ReadUInt32();

                DXTColor(coldat, ref colors);

                colors[2].blue  = (byte)((2 * colors[0].blue + colors[1].blue + 1) / 3);
                colors[2].green = (byte)((2 * colors[0].green + colors[1].green + 1) / 3);
                colors[2].red   = (byte)((2 * colors[0].red + colors[1].red + 1) / 3);

                colors[3].blue  = (byte)((colors[0].blue + 2 * colors[1].blue + 1) / 3);
                colors[3].green = (byte)((colors[0].green + 2 * colors[1].green + 1) / 3);
                colors[3].red   = (byte)((colors[0].red + 2 * colors[1].red + 1) / 3);

                for (var j = 0; j < 4; j++)
                {
                    var word = (ushort)(alphas[2 * j] | (alphas[2 * j + 1] << 8));
                    for (var i = 0; i < 4; i++)
                    {
                        var b = (byte)(word & 0x0F);
                        block[j * 4 + i] =   (byte)(b | (b << 4));
                        word             >>= 4;
                    }
                }

                for (int j = 0, k = 0; j < 4; j++)
                {
                    var y = iterHeight + j;
                    if (y >= height)
                    {
                        break;
                    }

                    for (var i = 0; i < 4; k++, i++)
                    {
                        var x = iterWidth + i;
                        if (x >= width)
                        {
                            break;
                        }

                        var sel    = (int)((bitmask & (0x03 << (k * 2))) >> (k * 2));
                        var col    = colors[sel];
                        var offset = (uint)(y * stride + x * 4);

                        result[offset]     = col.red;
                        result[offset + 1] = col.green;
                        result[offset + 2] = col.blue;
                        result[offset + 3] = block[k];
                    }
                }
            }
        }

        return result;
    }

    public override void InitializeFromHeader(in DDSHeader header)
    {
    }
}
using ZweigEngine.Image.DDS.Structures;

namespace ZweigEngine.Image.DDS.Decoder;

internal class DDSCompressedDXT1Decoder : DDSCompressedDecoder
{
    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        var colors = new DDSColour8888[4];
        var result = new byte[width * height * 4];
        var stride = width * 4;
        using var stream = new MemoryStream(input);
        using var reader = new BinaryReader(stream);

        colors[0].alpha = 0xFF;
        colors[1].alpha = 0xFF;
        colors[2].alpha = 0xFF;

        for (var iterHeight = 0u; iterHeight < height; iterHeight += 4)
        {
            for (var iterWidth = 0u; iterWidth < width; iterWidth += 4)
            {
                var coldat1 = reader.ReadUInt16();
                var coldat2 = reader.ReadUInt16();
                DXTColor(coldat1, ref colors[0]);
                DXTColor(coldat2, ref colors[1]);

                var bitmask = reader.ReadUInt32();
                if (coldat1 > coldat2)
                {
                    colors[2].blue = (byte)((2 * colors[0].blue + colors[1].blue + 1) / 3);
                    colors[2].green = (byte)((2 * colors[0].green + colors[1].green + 1) / 3);
                    colors[2].red = (byte)((2 * colors[0].red + colors[1].red + 1) / 3);

                    colors[3].blue = (byte)((colors[0].blue + 2 * colors[1].blue + 1) / 3);
                    colors[3].green = (byte)((colors[0].green + 2 * colors[1].green + 1) / 3);
                    colors[3].red = (byte)((colors[0].red + 2 * colors[1].red + 1) / 3);
                    colors[3].alpha = 0xFF;
                }
                else
                {
                    colors[2].blue = (byte)((colors[0].blue + colors[1].blue) / 2);
                    colors[2].green = (byte)((colors[0].green + colors[1].green) / 2);
                    colors[2].red = (byte)((colors[0].red + colors[1].red) / 2);

                    colors[3].blue = (byte)((colors[0].blue + 2 * colors[1].blue + 1) / 3);
                    colors[3].green = (byte)((colors[0].green + 2 * colors[1].green + 1) / 3);
                    colors[3].red = (byte)((colors[0].red + 2 * colors[1].red + 1) / 3);
                    colors[3].alpha = 0x00;
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

                        var select = (int)((bitmask & (0x03 << (k * 2))) >> (k * 2));
                        var col = colors[select];
                        var offset = (uint)(y * stride + x * 4);
                        result[offset + 0] = col.red;
                        result[offset + 1] = col.green;
                        result[offset + 2] = col.blue;
                        result[offset + 3] = col.alpha;
                    }
                }
            }
        }

        return result;
    }

    public override ulong GetDataSize(in uint width, in uint height)
    {
        return base.GetDataSize(width, height) / 2;
    }

    public override void InitializeFromHeader(in DDSHeader header)
    {
    }
}
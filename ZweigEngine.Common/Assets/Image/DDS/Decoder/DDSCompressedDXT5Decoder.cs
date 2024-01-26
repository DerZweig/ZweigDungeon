using ZweigEngine.Common.Assets.Image.DDS.Structures;

namespace ZweigEngine.Common.Assets.Image.DDS.Decoder;

internal class DDSCompressedDXT5Decoder : DDSCompressedDecoder
{
    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        var       block  = new byte[16];
        var       colors = new DDSColour8888[4];
        var       alphas = new ushort[8];
        var       result = new byte[width * height * 4];
        var       stride = width * 4;
        using var stream = new MemoryStream(input);
        using var reader = new BinaryReader(stream);

        for (var iterHeight = 0u; iterHeight < height; iterHeight += 4)
        {
            for (var iterWidth = 0u; iterWidth < width; iterWidth += 4)
            {
                alphas[0] = reader.ReadByte();
                alphas[1] = reader.ReadByte();
                var alphamask = reader.ReadBytes(6);
                var coldat    = reader.ReadBytes(4);
                var bitmask   = reader.ReadUInt32();

                DXTColor(coldat, ref colors);

                colors[2].blue  = (byte)((2 * colors[0].blue + colors[1].blue + 1) / 3);
                colors[2].green = (byte)((2 * colors[0].green + colors[1].green + 1) / 3);
                colors[2].red   = (byte)((2 * colors[0].red + colors[1].red + 1) / 3);
                
                colors[3].blue  = (byte)((colors[0].blue + 2 * colors[1].blue + 1) / 3);
                colors[3].green = (byte)((colors[0].green + 2 * colors[1].green + 1) / 3);
                colors[3].red   = (byte)((colors[0].red + 2 * colors[1].red + 1) / 3);

                if (alphas[0] > alphas[1])
                {
                    alphas[2] = (ushort)((6 * alphas[0] + 1 * alphas[1] + 3) / 7);
                    alphas[3] = (ushort)((5 * alphas[0] + 2 * alphas[1] + 3) / 7);
                    alphas[4] = (ushort)((4 * alphas[0] + 3 * alphas[1] + 3) / 7);
                    alphas[5] = (ushort)((3 * alphas[0] + 4 * alphas[1] + 3) / 7);
                    alphas[6] = (ushort)((2 * alphas[0] + 5 * alphas[1] + 3) / 7);
                    alphas[7] = (ushort)((1 * alphas[0] + 6 * alphas[1] + 3) / 7);
                }
                else
                {
                    alphas[2] = (ushort)((4 * alphas[0] + 1 * alphas[1] + 2) / 5);
                    alphas[3] = (ushort)((3 * alphas[0] + 2 * alphas[1] + 2) / 5);
                    alphas[4] = (ushort)((2 * alphas[0] + 3 * alphas[1] + 2) / 5);
                    alphas[5] = (ushort)((1 * alphas[0] + 4 * alphas[1] + 2) / 5);
                    alphas[6] = 0x00;
                    alphas[7] = 0xFF;
                }

                var bits1 = (uint)(alphamask[0] | (alphamask[1] << 8) | (alphamask[2] << 16));
                for (var j = 0; j < 2; j++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        block[j * 4 + i] = (byte)alphas[bits1 & 0x07];
                        bits1 >>= 3;
                    }
                }

                var bits2 = (uint)(alphamask[3] | (alphamask[4] << 8) | (alphamask[5] << 16));
                for (var j = 2; j < 4; j++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        block[j * 4 + i] = (byte)alphas[bits2 & 0x07];
                        bits2 >>= 3;
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
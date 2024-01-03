using ZweigEngine.Image.DDS.Structures;

namespace ZweigEngine.Image.DDS.Decoder;

internal class DDSUncompressedRGBADecoder : DDSUncompressedDecoder
{
    private readonly int[]  shift1  = new int[4];
    private readonly int[]  mul     = new int[4];
    private readonly int[]  shift2  = new int[4];
    private readonly uint[] bitmask = new uint[4];

    public override void InitializeFromHeader(in DDSHeader header)
    {
        bitmask[0] = header.PixelFormat.BBitMask;
        bitmask[1] = header.PixelFormat.GBitMask;
        bitmask[2] = header.PixelFormat.RBitMask;
        bitmask[3] = header.PixelFormat.ABitMask;

        ComputeMaskParams(bitmask[0], out shift1[0], out mul[0], out shift2[0]);
        ComputeMaskParams(bitmask[1], out shift1[1], out mul[1], out shift2[1]);
        ComputeMaskParams(bitmask[2], out shift1[2], out mul[2], out shift2[2]);
        ComputeMaskParams(bitmask[3], out shift1[3], out mul[3], out shift2[3]);
    }

    public override ulong GetDataSize(in uint width, in uint height)
    {
        return width * height * 4;
    }

    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        var stride = width * 4;
        var result = new byte[stride * height];

        using var stream = new MemoryStream(input);
        using var reader = new BinaryReader(stream);

        for (var iterHeight = 0u; iterHeight < height; ++iterHeight)
        {
            var offset = iterHeight * stride;

            for (var iterWidth = 0u; iterWidth < width; ++iterWidth)
            {
                var pixel = reader.ReadUInt32();
                var b     = pixel & bitmask[0];
                var g     = pixel & bitmask[1];
                var r     = pixel & bitmask[2];
                var a     = pixel & bitmask[3];

                result[offset++] = (byte)(((b >> shift1[0]) * mul[0]) >> shift2[0]);
                result[offset++] = (byte)(((g >> shift1[1]) * mul[1]) >> shift2[1]);
                result[offset++] = (byte)(((r >> shift1[2]) * mul[2]) >> shift2[2]);
                result[offset++] = (byte)(((a >> shift1[3]) * mul[3]) >> shift2[3]);
            }
        }

        return result;
    }
}
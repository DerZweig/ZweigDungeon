using System.Runtime.InteropServices;

namespace ZweigEngine.Image.DDS.Decoder;

internal abstract class DDSCompressedDecoder : DDSDecoder
{
    [StructLayout(LayoutKind.Sequential)]
    protected struct DDSColour8888
    {
        public byte red;
        public byte green;
        public byte blue;
        public byte alpha;
    }

    public override ulong GetDataSize(in uint width, in uint height)
    {
        var blockWidth  = Math.Max(1, (width + 3) / 4);
        var blockHeight = Math.Max(1, (height + 3) / 4);
        var blockTotal  = blockWidth * blockHeight;
        return blockTotal * 16;
    }

    protected static void DXTColor(ushort data, ref DDSColour8888 op)
    {
        var b = (byte)(data & 0x1f);
        var g = (byte)((data & 0x7E0) >> 5);
        var r = (byte)((data & 0xF800) >> 11);

        op.red   = (byte)(r << 3 | r >> 2);
        op.green = (byte)(g << 2 | g >> 3);
        op.blue  = (byte)(b << 3 | r >> 2);
    }

    protected static void DXTColor(IReadOnlyList<byte> data, ref DDSColour8888[] col)
    {
        var b0 = (byte)(data[0] & 0x1F);
        var g0 = (byte)(((data[0] & 0xE0) >> 5) | ((data[1] & 0x7) << 3));
        var r0 = (byte)((data[1] & 0xF8) >> 3);

        var b1 = (byte)(data[2] & 0x1F);
        var g1 = (byte)(((data[2] & 0xE0) >> 5) | ((data[3] & 0x7) << 3));
        var r1 = (byte)((data[3] & 0xF8) >> 3);

        col[0].red   = (byte)((r0 << 3) | (r0 >> 2));
        col[0].green = (byte)((g0 << 2) | (g0 >> 3));
        col[0].blue  = (byte)((b0 << 3) | (b0 >> 2));

        col[1].red   = (byte)((r1 << 3) | (r1 >> 2));
        col[1].green = (byte)((g1 << 2) | (g1 >> 3));
        col[1].blue  = (byte)((b1 << 3) | (b1 >> 2));
    }

    protected static void DXTPremult(uint pixnum, byte[] buffer)
    {
        for (uint i = 0; i < pixnum; i+= 4)
        {
            var alpha = buffer[i + 3];
            if (alpha == 0)
            {
                continue;
            }

            var red   = ((uint)buffer[i] << 8) / alpha;
            var green = ((uint)buffer[i + 1] << 8) / alpha;
            var blue  = ((uint)buffer[i + 2] << 8) / alpha;

            buffer[i]     = (byte)red;
            buffer[i + 1] = (byte)green;
            buffer[i + 2] = (byte)blue;
        }
    }
}
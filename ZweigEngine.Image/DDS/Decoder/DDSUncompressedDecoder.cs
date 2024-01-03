namespace ZweigEngine.Image.DDS.Decoder;

internal abstract class DDSUncompressedDecoder : DDSDecoder
{
    protected static void ComputeMaskParams(uint mask, out int shift1, out int mul, out int shift2)
    {
        shift1 = 0;
        mul    = 1;
        shift2 = 0;
        while ((mask & 1) == 0)
        {
            mask >>= 1;
            shift1++;
        }

        uint bc = 0;
        while ((mask & (1 << (int)bc)) != 0)
        {
            bc++;
        }

        while (mask * mul < 255)
        {
            mul = (mul << (int)bc) + 1;
        }

        mask *= (uint)mul;

        while ((mask & ~0xff) != 0)
        {
            mask >>= 1;
            shift2++;
        }
    }
}
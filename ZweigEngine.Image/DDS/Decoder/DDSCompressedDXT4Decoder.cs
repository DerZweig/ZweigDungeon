namespace ZweigEngine.Image.DDS.Decoder;

internal class DDSCompressedDXT4Decoder : DDSCompressedDXT5Decoder
{
    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        var result = base.DecodeData(input, in width, in height);
        DXTPremult(width * height, result);
        return result;
    }
}
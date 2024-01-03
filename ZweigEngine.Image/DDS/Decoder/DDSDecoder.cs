using ZweigEngine.Image.DDS.Structures;

namespace ZweigEngine.Image.DDS.Decoder;

internal abstract class DDSDecoder
{
    public abstract byte[] DecodeData(byte[] input, in uint width, in uint height);

    public abstract ulong GetDataSize(in uint width, in uint height);

    public abstract void InitializeFromHeader(in DDSHeader header);
}
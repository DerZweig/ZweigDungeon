﻿using ZweigDungeon.Common.Assets.Images.DDS.Structures;

namespace ZweigDungeon.Common.Assets.Images.DDS.Decoder;

internal class DDSUncompressedLuminanceDecoder : DDSUncompressedDecoder
{
    private uint bitcount;

    public override void InitializeFromHeader(in DDSHeader header)
    {
        bitcount = header.PixelFormat.RGBBitCount;
    }

    public override ulong GetDataSize(in uint width, in uint height)
    {
        return width * height * bitcount / 8;
    }

    public override byte[] DecodeData(byte[] input, in uint width, in uint height)
    {
        return input;
    }
}
namespace ZweigEngine.Common.Services.Video;

public interface IVideoLayer
{
    void Clear();
    void PutPixel(ushort x, ushort y, in VideoColor color);
}
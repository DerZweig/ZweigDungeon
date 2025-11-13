namespace ZweigEngine.Common.Video;

public interface IColorBuffer
{
    void Fill(in VideoColor color);
    void PutPixel(int x, int y, in VideoColor color);
}
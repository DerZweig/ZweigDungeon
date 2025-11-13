using ZweigDungeon.Services.Video;

namespace ZweigDungeon.Services.Platform;

internal interface IPlatform
{
    bool Initialize();
    bool ProcessInput();
    void SwapBuffers(in PixelBuffer buffer);
}
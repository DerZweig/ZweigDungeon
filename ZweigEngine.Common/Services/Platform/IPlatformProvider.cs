using ZweigEngine.Common.Services.Video;

namespace ZweigEngine.Common.Services.Platform;

public interface IPlatformProvider
{
    bool Start(string title, string version, string identifier);
    bool ProcessInput(CancellationToken cancellationToken);
    void SwapBuffers();

    int              VideoWidth  { get; }
    int              VideoHeight { get; }
    IColorBuffer     VideoBuffer { get; }
    IKeyboardDevice? Keyboard    { get; }
    IMouseDevice?    Mouse       { get; }
}
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon;

public class Application
{
    private readonly IVideoScreen m_screen;

    public Application(IVideoScreen screen)
    {
        m_screen = screen;
    }

    public static void Configure(IServiceConfiguration config)
    {
        config.AddSingleton<Application>();
    }

    public void Update(int width, int height)
    {
        m_screen.Foreground.PutPixel(0, 0, new VideoColor { Red = 255, Alpha = 255 });
    }
}
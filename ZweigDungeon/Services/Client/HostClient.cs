using ZweigEngine.Common.Services;
using ZweigEngine.Common.Video;

namespace ZweigDungeon.Services.Client;

internal sealed class HostClient
{
    private readonly IGlobalVariables m_globals;
    private readonly IVideoScreen     m_screen;

    public HostClient(IGlobalVariables globals, IVideoScreen screen)
    {
        m_globals = globals;
        m_screen  = screen;
    }

    public void UpdateScreen()
    {
        //var green = new VideoColor { Red = 128, Green = 255, Blue = 128, Alpha = 255 };
        var white = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        //var black = new VideoColor { Red = 0, Green   = 0, Blue   = 0, Alpha   = 255 };

        m_screen.Background.Fill(white);
    }
}
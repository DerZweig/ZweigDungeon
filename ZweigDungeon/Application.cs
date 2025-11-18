using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon;

internal sealed class Application
{
    private readonly IGlobalVariables m_globals;

    public Application(IGlobalVariables globals)
    {
        m_globals = globals;
    }

    public void UpdateScreen(IColorBuffer colorBuffer)
    {
        //var green = new VideoColor { Red = 128, Green = 255, Blue = 128, Alpha = 255 };
        var white = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
        var black = new VideoColor { Red = 0, Green   = 0, Blue   = 0, Alpha   = 255 };
        colorBuffer.Fill(black);

        if (m_globals.Mouse is { IsInsideScreen: true })
        {
            colorBuffer.PutPixel(m_globals.Mouse.PositionLeft, m_globals.Mouse.PositionTop, white);
            colorBuffer.PutPixel(m_globals.Mouse.PositionLeft + 1, m_globals.Mouse.PositionTop, white);
            colorBuffer.PutPixel(m_globals.Mouse.PositionLeft, m_globals.Mouse.PositionTop + 1, white);
        }
    }
}
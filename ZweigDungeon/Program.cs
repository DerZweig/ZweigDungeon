using SDL3;
using ZweigDungeon.Video;
using ZweigEngine.Common.Services;

namespace ZweigDungeon;

internal static class Program
{
    private const string AppTitle         = "ZweigDungeon";
    private const string AppVersion       = "0.1.0";
    private const string AppName          = "com.zweig.dungeon";
    private const int    AppDefaultWidth  = 800;
    private const int    AppDefaultHeight = 600;
    private const int    VideoBufferPitch = 640;

    [STAThread]
    private static void Main()
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            return;
        }

        SDL.SetAppMetadata(AppTitle, AppVersion, AppName);
        try
        {
            using (var surface = new VideoSurface(VideoBufferPitch, VideoBufferPitch))
            using (var window = new SDLWindow(AppTitle, AppDefaultWidth, AppDefaultHeight))
            using (var renderer = new SDLRenderer(window, null))
            using (var texture = new SDLTexture(renderer, VideoBufferPitch, VideoBufferPitch))
            using (var host = ServiceProviderHost.Create(ConfigureServices))
            {
                while (true)
                {
                    while (SDL.PollEvent(out var e))
                    {
                        switch (e.Type)
                        {
                            case (uint)SDL.EventType.Quit:
                                return;
                        }
                    }

                    var w = VideoBufferPitch;
                    var h = VideoBufferPitch;

                    if (renderer.GetOutputSize(out var vw, out var vh))
                    {
                        if (vw >= vh)
                        {
                            h = (ushort)(h * ((float)vh / vw));
                        }
                        else
                        {
                            w = (ushort)(w * ((float)vw / vh));
                        }
                    }

                    texture.Upload(surface);

                    //do the actual rendering 
                    renderer.Clear(0, 0, 0, 0);
                    renderer.DrawScreen(texture, w, h);
                    renderer.Present();
                }
            }
        }
        catch (Exception ex)
        {
            SDL.ShowSimpleMessageBox(SDL.MessageBoxFlags.Error, "Fatal Error", ex.Message, IntPtr.Zero);
        }
        finally
        {
            SDL.Quit();
        }
    }

    private static void ConfigureServices(IServiceConfiguration config)
    {
    }
}
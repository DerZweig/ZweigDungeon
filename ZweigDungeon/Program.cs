using System.Reflection;
using ZweigDungeon.Services;
using ZweigDungeon.Services.Client;
using ZweigDungeon.Services.Platform;
using ZweigDungeon.Services.Video;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.ServiceProvider;
using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon;

internal static class Program
{
    private const string AppTitle       = "ZweigDungeon";
    private const string AppIdentifier  = "com.zweig.dungeon";
    private const string AppLogLocation = "App";

    private static void Main()
    {
        using var host = ScopedServiceProvider.Create(config =>
        {
            config.AddFactory(() => new FileLogger("current.log", TimeSpan.FromMilliseconds(1500)));
            config.AddFactory(() =>
            {
                var version = Assembly.GetAssembly(typeof(Program))!.GetName().Version!;
                return new GlobalVariables
                {
                    AppTitle      = AppTitle,
                    AppIdentifier = AppIdentifier,
                    AppVersion    = version.ToString()
                };
            });

            config.AddSingleton<ILogger, FileLogger>();
            config.AddSingleton<IGlobalVariables, GlobalVariables>();
            config.AddSingleton<IGlobalCancellation, GlobalCancellation>();
        });

        var logger  = host.GetRequiredService<ILogger>();
        var globals = host.GetRequiredService<GlobalVariables>();

        logger.Info(AppLogLocation, $"Version {globals.AppVersion}");
        logger.Info(AppLogLocation, "Starting ...");
        try
        {
            using var client = ScopedServiceProvider.Create(host, config =>
            {
                config.AddSingleton<PixelBuffer>();
                config.AddSingleton<IColorBuffer, ColorBuffer>();
                config.AddSingleton<SDLDesktop>();
                config.AddSingleton<HostClient>();
            });

            var platform    = client.GetRequiredService<SDLDesktop>();
            var user        = client.GetRequiredService<HostClient>();
            var colorPixels = client.GetRequiredService<PixelBuffer>();
            var colorBuffer = client.GetRequiredService<IColorBuffer>();

            if (!platform.Initialize())
            {
                return;
            }

            globals.FrameClockLocal = DateTime.MinValue;
            globals.FrameClockUtc   = DateTime.MinValue;
            globals.FrameDeltaTime  = TimeSpan.Zero;

            while (platform.ProcessInput())
            {
                var now = DateTime.UtcNow;
                if (globals.FrameClockUtc > DateTime.MinValue)
                {
                    var tmp = now - globals.FrameClockUtc;
                    globals.FrameDeltaTime = tmp;
                }

                globals.FrameClockUtc   = now;
                globals.FrameClockLocal = now.ToLocalTime();

                user.UpdateScreen(colorBuffer);
                platform.SwapBuffers(colorPixels);
            }
        }
        catch (Exception ex)
        {
            logger.Error(AppLogLocation, ex.ToString());
        }
    }
}
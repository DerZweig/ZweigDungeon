using System.Reflection;
using ZweigDungeon.Services;
using ZweigDungeon.Services.Client;
using ZweigDungeon.Services.Platform;
using ZweigDungeon.Services.Video;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.ServiceProvider;
using ZweigEngine.Common.Video;

namespace ZweigDungeon;

internal static class Program
{
    private const string AppTitle       = "ZweigDungeon";
    private const string AppIdentifier  = "com.zweig.dungeon";
    private const string AppLogLocation = "App";

    private static void Main()
    {
        using var host = ServiceProviderHost.Create(config =>
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
            using var client = ServiceProviderHost.Create(host, config =>
            {
                config.AddSingleton<PixelBuffer>();
                config.AddSingleton<IColorBuffer, ColorBuffer>();
                config.AddSingleton<IPlatform, SDLDesktop>();
                config.AddSingleton<HostClient>();
            });

            var platform    = client.GetRequiredService<IPlatform>();
            var user        = client.GetRequiredService<HostClient>();
            var colorPixels = client.GetRequiredService<PixelBuffer>();
            var colorBuffer = client.GetRequiredService<IColorBuffer>();

            if (!platform.Initialize())
            {
                return;
            }

            while (platform.ProcessInput())
            {
                var now = DateTime.UtcNow;
                globals.FrameClockUtc   = now;
                globals.FrameClockLocal = now.ToLocalTime();

                user.UpdateScreen(colorBuffer);
                platform.SwapBuffers(colorPixels);
            }
        }
        catch (Exception ex)
        {
            logger.Error(AppLogLocation, "Unhandled error ", ex);
        }
    }
}
using System.Reflection;
using ZweigDungeon.Services;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Platform;
using ZweigEngine.Common.Services.ServiceProvider;
using ZweigEngine.Common.Services.Video;
using ZweigEngine.Platform.SDL;

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

        var logger       = host.GetRequiredService<ILogger>();
        var globals      = host.GetRequiredService<GlobalVariables>();
        var cancellation = host.GetRequiredService<GlobalCancellation>();

        logger.Info(AppLogLocation, $"Version {globals.AppVersion}");
        logger.Info(AppLogLocation, "Starting ...");
        try
        {
            using var client = ScopedServiceProvider.Create(host, config =>
            {
                config.AddSingleton<IPlatformProvider, SDLDesktop>();
                config.AddSingleton<Application>();
            });

            var platform = client.GetRequiredService<IPlatformProvider>();
            var user     = client.GetRequiredService<Application>();

            if (!platform.Start(globals.AppTitle, globals.AppVersion, globals.AppVersion))
            {
                return;
            }

            globals.FrameClockLocal = DateTime.MinValue;
            globals.FrameClockUtc   = DateTime.MinValue;
            globals.FrameDeltaTime  = TimeSpan.Zero;

            try
            {
                while (platform.ProcessInput(cancellation.Token))
                {
                    var now = DateTime.UtcNow;
                    globals.Keyboard        = platform.Keyboard;
                    globals.Mouse           = platform.Mouse;
                    globals.ScreenWidth     = platform.VideoWidth;
                    globals.ScreenHeight    = platform.VideoHeight;
                    globals.FrameClockUtc   = now;
                    globals.FrameClockLocal = now.ToLocalTime();
                    if (globals.FrameClockUtc > DateTime.MinValue)
                    {
                        var tmp = now - globals.FrameClockUtc;
                        globals.FrameDeltaTime = tmp;
                    }

                    user.UpdateScreen(platform.VideoBuffer);
                    platform.SwapBuffers();
                }
            }
            finally
            {
                cancellation.Cancel();
            }
        }
        catch (Exception ex)
        {
            logger.Error(AppLogLocation, ex.ToString());
        }
    }
}
using SDL3;
using System.Reflection;
using ZweigDungeon.Services;
using ZweigDungeon.Services.Video;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Video;

namespace ZweigDungeon;

internal static class Program
{
    private const int DefaultWindowWidth  = 800;
    private const int DefaultWindowHeight = 600;
    private const int MinimumOutputWidth  = 640;
    private const int MinimumOutputHeight = 480;

    private const string AppTitle      = "ZweigDungeon";
    private const string AppIdentifier = "com.zweig.dungeon";


    [STAThread]
    private static void Main(string[] args)
    {
        using var host = ServiceProviderHost.Create(config =>
        {
            config.AddFactory<ILogger>(() => new FileLogger("current.log", TimeSpan.FromMilliseconds(1500)));
            config.AddFactory<IApplicationMetaData>(() =>
            {
                var version = Assembly.GetAssembly(typeof(Program))!.GetName().Version!;
                return new ApplicationMetaData(AppTitle, version, args);
            });
            config.AddSingleton<IGlobalCancellation, GlobalCancellation>();
            config.AddSingleton<PixelScreen>();
            config.AddSingleton<IVideoScreen, VideoScreen>();
        });

        var logger       = host.GetRequiredService<ILogger>();
        var meta         = host.GetRequiredService<IApplicationMetaData>();
        var cancellation = host.GetRequiredService<IGlobalCancellation>().Token;
        var screen       = host.GetRequiredService<PixelScreen>();

        var window     = IntPtr.Zero;
        var renderer   = IntPtr.Zero;
        var background = IntPtr.Zero;
        var foreground = IntPtr.Zero;
        var menu       = IntPtr.Zero;

        logger.Info(nameof(SDL), $"Version {meta.Version}");
        logger.Info(nameof(SDL), "Initialize SDL");
        if (!SDL.SetAppMetadata(nameof(SDL), meta.Version, AppIdentifier) ||
            !SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            logger.Error(nameof(SDL), $"Failed to initialize SDL {SDL.GetError()}");
            return;
        }


        try
        {
            logger.Info(nameof(SDL), "Creating window");
            if (renderer != IntPtr.Zero || window != IntPtr.Zero)
            {
                logger.Error(nameof(SDL), "Window already created");
                return;
            }

            if (!SDL.CreateWindowAndRenderer(meta.Title,
                                             DefaultWindowWidth,
                                             DefaultWindowHeight,
                                             SDL.WindowFlags.Hidden,
                                             out window,
                                             out renderer))
            {
                logger.Error(nameof(SDL), "Failed to create window");
                return;
            }

            if (!SDL.SetWindowBordered(window, true) ||
                !SDL.SetWindowResizable(window, true))
            {
                logger.Error(nameof(SDL), "Failed to configure window");
                return;
            }

            if (!SDL.SetRenderVSync(renderer, 1) ||
                !SDL.SetRenderTextureAddressMode(renderer, SDL.TextureAddressMode.Wrap, SDL.TextureAddressMode.Wrap) ||
                !SDL.SetDefaultTextureScaleMode(renderer, SDL.ScaleMode.Nearest))
            {
                logger.Error(nameof(SDL), "Failed to configure renderer");
                return;
            }

            if (!TryCreateScreenTexture(renderer, PixelScreen.Width, PixelScreen.Height, out menu) ||
                !TryCreateScreenTexture(renderer, PixelScreen.Width, PixelScreen.Height, out foreground) ||
                !TryCreateScreenTexture(renderer, PixelScreen.Width, PixelScreen.Height, out background))
            {
                logger.Error(nameof(SDL), "Failed to create render targets");
                return;
            }

            if (!SDL.ShowWindow(window))
            {
                logger.Error(nameof(SDL), "Failed to show window");
                return;
            }


            using (var client = ServiceProviderHost.Create(host, Application.Configure))
            {
                var app = client.GetRequiredService<Application>();
                while (!cancellation.IsCancellationRequested)
                {
                    if (SDL.PollEvent(out var e))
                    {
                        if (e.Type == (uint)SDL.EventType.Quit)
                        {
                            return;
                        }
                    }

                    if (!SDL.GetRenderOutputSize(renderer, out var videoWidth, out var videoHeight))
                    {
                        logger.Error(nameof(SDL), $"Failed to query renderer size {SDL.GetError()}");
                    }

                    videoWidth  = Math.Max(videoWidth, MinimumOutputWidth);
                    videoHeight = Math.Max(videoHeight, MinimumOutputHeight);

                    var scaleWidth  = 1.0f;
                    var scaleHeight = 1.0f;
                    if (videoWidth >= videoHeight)
                    {
                        scaleHeight = (float)videoHeight / videoWidth;
                    }
                    else
                    {
                        scaleWidth = (float)videoWidth / videoHeight;
                    }

                    var clientWidth  = (int)(scaleWidth * PixelScreen.Width);
                    var clientHeight = (int)(scaleHeight * PixelScreen.Height);

                    var srcRect = new SDL.FRect { X = 0.0f, Y = 0.0f, W = clientWidth, H = clientHeight };
                    var dstRect = new SDL.FRect { X = 0.0f, Y = 0.0f, W = videoWidth, H  = videoHeight };

                    app.Update(clientWidth, clientHeight);

                    if (!SDL.SetRenderDrawColor(renderer, 0, 0, 0, 0) ||
                        !SDL.RenderClear(renderer) ||
                        !SDL.UpdateTexture(background, IntPtr.Zero, screen.Background.Address, PixelScreen.Pitch) ||
                        !SDL.UpdateTexture(foreground, IntPtr.Zero, screen.Foreground.Address, PixelScreen.Pitch) ||
                        !SDL.UpdateTexture(menu, IntPtr.Zero, screen.Menu.Address, PixelScreen.Pitch) ||
                        !SDL.RenderTexture(renderer, background, srcRect, dstRect) ||
                        !SDL.RenderTexture(renderer, foreground, srcRect, dstRect) ||
                        !SDL.RenderTexture(renderer, menu, srcRect, dstRect) ||
                        !SDL.RenderPresent(renderer))
                    {
                        logger.Error(nameof(SDL), $"Failed to present screen {SDL.GetError()}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(AppTitle, $"Unhandled error {ex}");
        }
        finally
        {
            logger.Info(nameof(SDL), "Shutdown");
            ReleaseResource(ref menu, SDL.DestroyTexture);
            ReleaseResource(ref foreground, SDL.DestroyTexture);
            ReleaseResource(ref background, SDL.DestroyTexture);
            ReleaseResource(ref renderer, SDL.DestroyRenderer);
            ReleaseResource(ref window, SDL.DestroyWindow);
            SDL.Quit();
        }
    }

    private static void ReleaseResource(ref IntPtr ptr, Action<IntPtr> destroy)
    {
        if (ptr == IntPtr.Zero)
        {
            return;
        }

        destroy(ptr);
        ptr = IntPtr.Zero;
    }

    private static bool TryCreateScreenTexture(IntPtr renderer, int width, int height, out IntPtr texture)
    {
        var format = SDL.PixelFormat.ABGR8888;
        if (!BitConverter.IsLittleEndian)
        {
            format = SDL.PixelFormat.RGBA8888;
        }

        texture = SDL.CreateTexture(renderer, format, SDL.TextureAccess.Streaming, width, height);
        return texture != IntPtr.Zero;
    }
}
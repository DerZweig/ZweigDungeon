using SDL3;
using ZweigEngine.Common.Services;

namespace ZweigDungeon;

internal static class Program
{

    [STAThread]
    private static void Main()
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            SDL.LogError(SDL.LogCategory.System, $"SDL couldn't initialize: {SDL.GetError()}");
            return;
        }

        try
        {
            using (var host = ServiceProviderHost.Create(ConfigureServices))
            {
               
            }
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
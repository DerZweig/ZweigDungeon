
using System.Runtime.InteropServices;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services;
using ZweigDungeon.Common.Services.Libraries;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Native.OpenGL;
using ZweigDungeon.Native.Win32;

namespace ZweigDungeon.Application;

internal static class Program
{
	private static void Main(string[] args)
	{
		var serviceConfig = new ServiceConfiguration();
		serviceConfig.AddSingleton<MessageBus>();
		serviceConfig.AddSingleton<NativeLibraryLoader>();

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			serviceConfig.AddSingleton<IPlatformWindow, Win32Window>();
			serviceConfig.AddSingleton<IPlatformKeyboard, Win32Keyboard>();
			serviceConfig.AddSingleton<IPlatformMouse, Win32Mouse>();
			serviceConfig.AddSingleton<IPlatformAudio, Win32AudioDevice>();
			serviceConfig.AddSingleton<IPlatformVideo, Win32OpenGLDevice>();
			serviceConfig.AddSingleton<IVideoContext, OpenGLContext>();
		}
		else
		{
			throw new NotSupportedException("Platform is not implemented");
		}

		serviceConfig.AddSingleton<Game>();
		using (var services = serviceConfig.Build())
		{
			var window = services.GetRequiredService<IPlatformWindow>();
			window.Create();
			while (window.IsAvailable())
			{
				window.Update();
			}
		}
	}
}
using System.Diagnostics;
using System.Runtime.InteropServices;
using ZweigDungeon.Application.Manager.Implementation;
using ZweigDungeon.Application.Manager.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Messages;
using ZweigEngine.Native.OpenGL;
using ZweigEngine.Native.Win32;

namespace ZweigDungeon.Application;

internal static class Program
{
	private static void Main(string[] args)
	{
		try
		{
			var serviceConfig = new ServiceConfiguration();
			serviceConfig.AddSingleton<MessageBus>();
			serviceConfig.AddSingleton<NativeLibraryLoader>();
			serviceConfig.AddSingleton<IVideoContext, OpenGLContext>();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				serviceConfig.AddSingleton<IPlatformWindow, Win32Window>();
				serviceConfig.AddSingleton<IPlatformKeyboard, Win32Keyboard>();
				serviceConfig.AddSingleton<IPlatformMouse, Win32Mouse>();
				serviceConfig.AddSingleton<IPlatformAudio, Win32AudioDevice>();
				serviceConfig.AddSingleton<IPlatformVideo, Win32OpenGLDevice>();
			}
			else
			{
				throw new NotSupportedException("Platform is not implemented");
			}
			
			serviceConfig.AddSingleton<IImageManager, ImageManager>();
			serviceConfig.AddSingleton<IFontManager, FontManager>();
			serviceConfig.AddSingleton<App>();
			

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
		catch (Exception ex)
		{
			if (Debugger.IsAttached)
			{
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace ?? string.Empty);
			}
		}
	}
}
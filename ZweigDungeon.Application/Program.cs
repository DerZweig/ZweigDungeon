﻿
using System.Diagnostics;
using System.Runtime.InteropServices;
using ZweigDungeon.Application.Manager;
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


			serviceConfig.AddSingleton<FontManager>();
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
		catch(Exception ex)
		{
			if (Debugger.IsAttached)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
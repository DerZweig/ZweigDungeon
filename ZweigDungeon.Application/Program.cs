﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Services.Interfaces.Files;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Repositories;
using ZweigEngine.Common.Services.Video;
using ZweigEngine.Native.OpenGL;
using ZweigEngine.Native.Win32;
using ZweigEngine.Native.Win32.Interfaces;

namespace ZweigDungeon.Application;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		try
		{
			var serviceConfig = new ServiceConfiguration();
			serviceConfig.AddSingleton<NativeLibraryLoader>();
			serviceConfig.AddSingleton<IFileConfig, FileConfig>();
			serviceConfig.AddSingleton<ImageRepository>();
			serviceConfig.AddSingleton<FontRepository>();
			serviceConfig.AddSingleton<TextureRepository>();
			serviceConfig.AddSingleton<TileSheetRepository>();
			serviceConfig.AddSingleton<VideoContext>();

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				serviceConfig.AddSingleton<IWin32ProcessDpiScalingConfig, Win32UsePerMonitorDpiScalingConfig>();
				serviceConfig.AddSingleton<IPlatformSynchronization, Win32Synchronization>();
				serviceConfig.AddSingleton<IPlatformWindow, Win32Window>();
				serviceConfig.AddSingleton<IPlatformKeyboard, Win32Keyboard>();
				serviceConfig.AddSingleton<IPlatformMouse, Win32Mouse>();
				serviceConfig.AddSingleton<IPlatformAudio, Win32Audio>();
				serviceConfig.AddSingleton<IPlatformVideo, Win32Video>();
			}
			else
			{
				throw new NotSupportedException("Platform is not implemented");
			}

			var assembly = Assembly.GetExecutingAssembly();
			var types    = assembly.GetTypes();

			foreach (var type in types)
			{
				if (type.IsInterface || type.Namespace?.Contains("Application.Services.") == false)
				{
					continue;
				}

				var interfaceType = type.GetInterfaces().SingleOrDefault(x => x.Name.EndsWith(type.Name));
				if (interfaceType == null)
				{
					serviceConfig.AddSingleton(type);
				}
				else
				{
					serviceConfig.AddSingleton(interfaceType, type);
				}
			}

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
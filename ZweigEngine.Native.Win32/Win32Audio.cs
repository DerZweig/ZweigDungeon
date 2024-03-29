﻿using ZweigEngine.Common.Services.Interfaces.Platform;

namespace ZweigEngine.Native.Win32;

public class Win32Audio : IPlatformAudio
{
	public string GetDeviceName() => "Win32 MIDI Audio";
}
using ZweigEngine.Common.Interfaces.Platform;

namespace ZweigEngine.Native.Win32;

public class Win32AudioDevice : IPlatformAudio
{
	public string Name => "Win32 MIDI Audio";
}
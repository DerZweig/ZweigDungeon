using ZweigDungeon.Common.Interfaces.Platform;

namespace ZweigDungeon.Native.Win32;

public class Win32AudioDevice : IPlatformAudio
{
	public string Name => "Win32 MIDI Audio";
}
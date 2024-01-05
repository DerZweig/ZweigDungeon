namespace ZweigEngine.Common.Interfaces.Platform.Messages;

public interface IAudioDeviceListener
{
	void AudioDeviceActivated(IPlatformAudio audio);
	void AudioDeviceDeactivating(IPlatformAudio audio);
}
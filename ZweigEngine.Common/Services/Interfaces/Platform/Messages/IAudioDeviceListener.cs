namespace ZweigEngine.Common.Services.Interfaces.Platform.Messages;

public interface IAudioDeviceListener
{
	void AudioDeviceActivated(IPlatformAudio audio);
	void AudioDeviceDeactivating(IPlatformAudio audio);
}
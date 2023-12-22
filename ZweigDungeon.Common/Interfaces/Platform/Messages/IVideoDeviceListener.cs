namespace ZweigDungeon.Common.Interfaces.Platform.Messages;

public interface IVideoDeviceListener
{
	void VideoDeviceActivated(IPlatformVideo video);
	void VideoDeviceDeactivating(IPlatformVideo video);
}
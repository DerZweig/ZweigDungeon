namespace ZweigEngine.Common.Services.Interfaces.Platform;

public delegate void PlatformVideoDeviceDelegate(IPlatformVideoOutput video);

public interface IPlatformVideoOutput
{
	event PlatformVideoDeviceDelegate OnActivated;
	event PlatformVideoDeviceDelegate OnDeactivating;
	event PlatformVideoDeviceDelegate OnBeginFrame;
	event PlatformVideoDeviceDelegate OnFinishFrame;

	int GetViewportWidth();
	int GetViewportHeight();
}
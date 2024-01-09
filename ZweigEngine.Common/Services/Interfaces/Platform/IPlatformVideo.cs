namespace ZweigEngine.Common.Services.Interfaces.Platform;

public delegate void PlatformVideoDeviceDelegate(IPlatformVideo video);

public interface IPlatformVideo
{
	event PlatformVideoDeviceDelegate OnActivated;
	event PlatformVideoDeviceDelegate OnDeactivating;
	event PlatformVideoDeviceDelegate OnBeginFrame;
	event PlatformVideoDeviceDelegate OnFinishFrame;

	string GetDeviceName();
	int    GetViewportWidth();
	int    GetViewportHeight();
	
}
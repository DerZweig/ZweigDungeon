using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Services.Interfaces.Platform;

public delegate void PlatformVideoActivateDelegate(IPlatformVideo video, IVideoBackend backend);

public delegate void PlatformVideoDelegate(IPlatformVideo video);

public interface IPlatformVideo
{
	event PlatformVideoActivateDelegate OnActivated;
	event PlatformVideoDelegate         OnDeactivating;
	event PlatformVideoDelegate         OnBeginFrame;
	event PlatformVideoDelegate         OnFinishFrame;

	IEnumerable<IVideoDriver> EnumerateDrivers();

	void ConfigureSurface(IVideoDriver driver, in VideoDeviceDescription deviceDescription, Func<IVideoSurface, IVideoBackend> backendFactory);
}
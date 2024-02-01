using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Services.Video.Interfaces;

public interface IVideoDriver
{
	public string Name { get; }

	public IEnumerable<VideoDeviceDescription> EnumerateDevices();
}
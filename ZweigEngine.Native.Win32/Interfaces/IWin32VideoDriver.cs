using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Native.Win32.Interfaces;

internal interface IWin32VideoDriver : IVideoDriver
{
	IWin32VideoSurface CreateSurface(in VideoDeviceDescription device);
}
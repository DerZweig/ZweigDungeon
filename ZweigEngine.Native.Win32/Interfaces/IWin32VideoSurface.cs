using ZweigEngine.Common.Services.Video.Interfaces;

namespace ZweigEngine.Native.Win32.Interfaces;

internal interface IWin32VideoSurface : IVideoSurface
{
	void Resize(int width, int height);
	void Present();
}
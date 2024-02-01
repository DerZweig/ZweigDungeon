using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Native.Win32.Interfaces;

namespace ZweigEngine.Native.Win32.Video;

internal class Win32Direct3DVideoDriver : IDisposable, IWin32VideoDriver
{
	private readonly NativeLibraryLoader m_loader;
	private readonly Win32Window         m_window;

	public Win32Direct3DVideoDriver(NativeLibraryLoader loader, Win32Window window, string name)
	{
		m_loader = loader;
		m_window = window;
		Name     = name;
	}

	private void ReleaseUnmanagedResources()
	{
		// TODO release unmanaged resources here
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~Win32Direct3DVideoDriver()
	{
		ReleaseUnmanagedResources();
	}

	public string Name { get; }

	public IEnumerable<VideoDeviceDescription> EnumerateDevices()
	{
		throw new NotImplementedException();
	}

	public IWin32VideoSurface CreateSurface(in VideoDeviceDescription device)
	{
		throw new NotImplementedException();
	}
}
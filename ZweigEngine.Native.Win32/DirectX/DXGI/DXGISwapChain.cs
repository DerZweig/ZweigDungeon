using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGISwapChain : DirectXObject
{
	private delegate Win32HResult PfnPresentDelegate(IntPtr self, int syncInterval, DXGIPresentFlags flags);
	private delegate Win32HResult PfnGetBufferDelegate(IntPtr self, int buffer, Guid uuid, out IntPtr surface);
	private delegate Win32HResult PfnResizeBuffersDelegate(IntPtr self, int bufferCount, int width, int height, DXGIFormat newFormat, uint flags);

	private readonly PfnPresentDelegate       m_present;
	private readonly PfnGetBufferDelegate     m_getBuffer;
	private readonly PfnResizeBuffersDelegate m_resizeBuffers;
	
	
	internal DXGISwapChain(IntPtr pointer) : base(pointer)
	{
		LoadMethod(8u, out m_present);
		LoadMethod(9u, out m_getBuffer);
		LoadMethod(13u, out m_resizeBuffers);
	}

	public bool TryPresent(int syncInterval, DXGIPresentFlags flags)
	{
		return m_present(Self, syncInterval, flags).Success;
	}

	public bool TryResizeBuffers(int width, int height, DXGIFormat newFormat)
	{
		return m_resizeBuffers(Self, 0, width, height, newFormat, 0u).Success;
	}
}
using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGISwapChain : DXObject
{
	private delegate Win32HResult PfnPresentDelegate(IntPtr self, int syncInterval, DXGIPresentFlags flags);
	private delegate Win32HResult PfnGetBufferDelegate(IntPtr self, int buffer, IntPtr uuid, out IntPtr surface);
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

	public bool TryGetBuffer(int buffer, ref DXGISurface? surface)
	{
		surface?.Dispose();
		surface = null;
		
		var       uuid   = new Guid("cafcb56c-6ac3-4889-bf47-9e23bbd260ec");
		using var pinned = new PinnedObject<Guid>(uuid, GCHandleType.Pinned);
		if (m_getBuffer(Self, buffer, pinned.GetAddress(), out var pointer).Success)
		{
			surface = new DXGISurface(pointer);
			return true;
		}

		return false;
	}
}
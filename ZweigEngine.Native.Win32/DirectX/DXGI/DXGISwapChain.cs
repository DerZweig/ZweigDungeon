using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.Win32.DirectX.D3D11;
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

	public bool TryGetBuffer(int buffer, ref D3D11Texture2D? texture)
	{
		texture?.Dispose();
		texture = null;
		
		var       uuid   = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
		using var pinned = new PinnedObject<Guid>(uuid, GCHandleType.Pinned);
		if (m_getBuffer(Self, buffer, pinned.GetAddress(), out var pointer).Success)
		{
			texture = new D3D11Texture2D(pointer);
			return true;
		}

		return false;
	}
}
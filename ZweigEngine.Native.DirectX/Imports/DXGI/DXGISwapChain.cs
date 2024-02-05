using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;
using ZweigEngine.Native.DirectX.Imports.Structures;
using ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

namespace ZweigEngine.Native.DirectX.Imports.DXGI;

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
		LoadMethod((in DXGISwapChainMethodTable table) => table.Present, out m_present);
		LoadMethod((in DXGISwapChainMethodTable table) => table.GetBuffer, out m_getBuffer);
		LoadMethod((in DXGISwapChainMethodTable table) => table.ResizeBuffers, out m_resizeBuffers);
	}

	public bool TryPresent(int syncInterval, DXGIPresentFlags flags)
	{
		return m_present(Self, syncInterval, flags).Success;
	}

	public bool TryResizeBuffers(int width, int height, DXGIFormat newFormat)
	{
		return m_resizeBuffers(Self, 0, width, height, newFormat, 0u).Success;
	}

	public bool TryGetBuffer<TResult>(int buffer, Guid uuid, ref TResult? surface) where TResult : DXObject
	{
		surface?.Dispose();
		surface = null;

		using var pinned = new PinnedObject<Guid>(uuid, GCHandleType.Pinned);
		if (m_getBuffer(Self, buffer, pinned.GetAddress(), out var pointer).Success)
		{
			surface = (TResult?)Activator.CreateInstance(typeof(TResult), pointer);
			return true;
		}

		return false;
	}
}
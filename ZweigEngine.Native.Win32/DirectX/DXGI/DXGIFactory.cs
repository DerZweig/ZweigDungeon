using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.Win32.DirectX.DXGI.Constants;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Prototypes;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGIFactory : DXObject
{
	private delegate Win32HResult PfnEnumAdaptersDelegate(IntPtr self, uint index, out IntPtr adapter);
	private delegate Win32HResult PfnMakeWindowAssociationDelegate(IntPtr self, IntPtr window, DXGIMakeWindowAssociationFlags flags);
	private delegate Win32HResult PfnCreateSwapChainDelegate(IntPtr self, IntPtr device, IntPtr desc, out IntPtr swapChain);

	private readonly PfnEnumAdaptersDelegate          m_enumAdapters;
	private readonly PfnMakeWindowAssociationDelegate m_makeWindowAssociation;
	private readonly PfnCreateSwapChainDelegate       m_createSwapChain;

	private DXGIFactory(IntPtr pointer) : base(pointer)
	{
		LoadMethod(7u, out m_enumAdapters);
		LoadMethod(8u, out m_makeWindowAssociation);
		LoadMethod(10u, out m_createSwapChain);
	}

	public static bool TryCreate(NativeLibraryLoader loader, out DXGIFactory factory)
	{
		factory = null!;

		var       uuid   = new Guid("7b7166ec-21c7-44ae-b21a-c9ae321ae369");
		using var pinned = new PinnedObject<Guid>(uuid, GCHandleType.Pinned);

		loader.LoadFunction("dxgi", "CreateDXGIFactory", out PfnCreateDXGIFactory construct);
		if (construct(pinned.GetAddress(), out var pointer).Success)
		{
			factory = new DXGIFactory(pointer);
			return true;
		}

		return false;
	}

	public bool TryEnumAdapters(uint index, ref DXGIAdapter? adapter)
	{
		adapter?.Dispose();
		adapter = null;
		if (m_enumAdapters(Self, index, out var result).Success)
		{
			adapter = new DXGIAdapter(result);
			return true;
		}

		return false;
	}

	public bool TryMakeWindowAssociation(IntPtr window, DXGIMakeWindowAssociationFlags flags)
	{
		return m_makeWindowAssociation(Self, window, flags).Success;
	}

	public bool CreateSwapChain(IntPtr device, in DXGISwapChainDescription description, out DXGISwapChain swapChain)
	{
		swapChain = null!;
		
		using var pinned = new PinnedObject<DXGISwapChainDescription>(description, GCHandleType.Pinned);
		if (m_createSwapChain(Self, device, pinned.GetAddress(), out var result).Success)
		{
			swapChain = new DXGISwapChain(result);
			return true;
		}

		return false;
	}
}
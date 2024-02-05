using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.DirectX.Imports.DXGI.Constants;
using ZweigEngine.Native.DirectX.Imports.DXGI.Structures;
using ZweigEngine.Native.DirectX.Imports.Prototypes;
using ZweigEngine.Native.DirectX.Imports.Structures;
using ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

namespace ZweigEngine.Native.DirectX.Imports.DXGI;

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
		LoadMethod((in DXGIFactoryMethodTable table) => table.EnumAdapters, out m_enumAdapters);
		LoadMethod((in DXGIFactoryMethodTable table) => table.MakeWindowAssociation, out m_makeWindowAssociation);
		LoadMethod((in DXGIFactoryMethodTable table) => table.CreateSwapChain, out m_createSwapChain);
	}

	public static bool TryCreate(NativeLibraryLoader loader, out DXGIFactory factory)
	{
		factory = null!;

		var       uuid   = typeof(DXGIFactoryMethodTable).GUID;
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
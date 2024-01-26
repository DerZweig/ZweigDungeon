using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGIAdapter : DirectXObject
{
	private delegate Win32HResult PfnGetDescDelegate(IntPtr self, IntPtr desc);

	private delegate Win32HResult PfnEnumOutputsDelegate(IntPtr self, uint index, out IntPtr output);

	private readonly PfnEnumOutputsDelegate m_enumOutputs;
	private readonly PfnGetDescDelegate     m_getDesc;

	internal DXGIAdapter(IntPtr pointer) : base(pointer)
	{
		LoadMethod(7u, out m_enumOutputs);
		LoadMethod(8u, out m_getDesc);
	}

	public bool TryEnumOutputs(uint index, out DXGIOutput output)
	{
		output = null!;
		if (m_enumOutputs(Self, index, out var result).Success)
		{
			output = new DXGIOutput(result);
			return true;
		}

		return false;
	}

	public bool TryGetDescription(out DXGIAdapterDescription desc)
	{
		var size    = Marshal.SizeOf<DXGIAdapterDescription>();
		var pointer = Marshal.AllocHGlobal(size);

		try
		{
			desc = default;
			if (m_getDesc.Invoke(Self, pointer).Success)
			{
				desc = Marshal.PtrToStructure<DXGIAdapterDescription>(pointer);
			}

			return false;
		}
		finally
		{
			Marshal.FreeHGlobal(pointer);
		}
	}
}
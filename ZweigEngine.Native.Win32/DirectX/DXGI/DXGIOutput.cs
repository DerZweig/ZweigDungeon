using System.Runtime.InteropServices;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGIOutput : DirectXObject
{
	private delegate Win32HResult PfnGetDescDelegate(IntPtr self, IntPtr desc);

	private readonly PfnGetDescDelegate m_getDesc;

	internal DXGIOutput(IntPtr pointer) : base(pointer)
	{
		
		LoadMethod(7u, out m_getDesc);
	}
	
	public bool TryGetDescription(out DXGIOutputDescription desc)
	{
		var size    = Marshal.SizeOf<DXGIOutputDescription>();
		var pointer = Marshal.AllocHGlobal(size);
		
		try
		{
			desc = default;
			if (m_getDesc.Invoke(Self, pointer).Success)
			{
				desc = Marshal.PtrToStructure<DXGIOutputDescription>(pointer);
				return true;
			}

			return false;
		}
		finally
		{
			Marshal.FreeHGlobal(pointer);
		}
	}
}
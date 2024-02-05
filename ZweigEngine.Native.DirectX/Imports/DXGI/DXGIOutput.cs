using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.DirectX.Imports.DXGI.Structures;
using ZweigEngine.Native.DirectX.Imports.Structures;
using ZweigEngine.Native.DirectX.Imports.VTables.DXGI;

namespace ZweigEngine.Native.DirectX.Imports.DXGI;

internal sealed class DXGIOutput : DXObject
{
	private delegate Win32HResult PfnGetDescDelegate(IntPtr self, ref DXGIOutputDescription desc);

	private delegate Win32HResult PfnFindClosestMatchingMode(IntPtr self, IntPtr modeToMatch, ref DXGIModeDescription closestMatch, IntPtr concernedDevice);

	private readonly PfnGetDescDelegate         m_getDesc;
	private readonly PfnFindClosestMatchingMode m_findClosestMatchingMode;

	internal DXGIOutput(IntPtr pointer) : base(pointer)
	{
		LoadMethod((in DXGIOutputMethodTable table) => table.GetDesc, out m_getDesc);
		LoadMethod((in DXGIOutputMethodTable table) => table.FindClosestMatchingMode, out m_findClosestMatchingMode);
	}

	public bool TryGetDescription(out DXGIOutputDescription desc)
	{
		desc = default;
		return m_getDesc(Self, ref desc).Success;
	}

	public bool TryFindClosestMatchingMode(in DXGIModeDescription modeToMatch, out DXGIModeDescription closestMatch)
	{
		closestMatch = default;
		using var pinned = new PinnedObject<DXGIModeDescription>(modeToMatch, GCHandleType.Pinned);
		return m_findClosestMatchingMode(Self, pinned.GetAddress(), ref closestMatch, IntPtr.Zero).Success;
	}
}
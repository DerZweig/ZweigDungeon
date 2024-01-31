using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.Win32.DirectX.DXGI.Structures;
using ZweigEngine.Native.Win32.Structures;

namespace ZweigEngine.Native.Win32.DirectX.DXGI;

internal sealed class DXGIOutput : DXObject
{
	private delegate Win32HResult PfnGetDescDelegate(IntPtr self, ref DXGIOutputDescription desc);
	private delegate Win32HResult PfnFindClosestMatchingMode(IntPtr self, IntPtr modeToMatch, ref DXGIModeDescription closestMatch, IntPtr concernedDevice);

	private readonly PfnGetDescDelegate         m_getDesc;
	private readonly PfnFindClosestMatchingMode m_findClosestMatchingMode;

	internal DXGIOutput(IntPtr pointer) : base(pointer)
	{
		LoadMethod(7u, out m_getDesc);
		LoadMethod(9u, out m_findClosestMatchingMode);
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
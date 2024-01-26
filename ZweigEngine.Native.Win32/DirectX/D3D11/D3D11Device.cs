using ZweigEngine.Common.Services.Libraries;
using ZweigEngine.Native.Win32.DirectX.Constants;
using ZweigEngine.Native.Win32.DirectX.D3D11.Constants;
using ZweigEngine.Native.Win32.DirectX.DXGI;
using ZweigEngine.Native.Win32.Prototypes;

namespace ZweigEngine.Native.Win32.DirectX.D3D11;

internal sealed class D3D11Device : DirectXObject
{
	public Direct3DFeatureLevel FeatureLevel { get; }

	private D3D11Device(IntPtr pointer, Direct3DFeatureLevel featureLevel) : base(pointer)
	{
		FeatureLevel = featureLevel;
	}

	public static bool TryCreate(NativeLibraryLoader loader, DXGIAdapter? adapter, Direct3DDriverType driverType, out D3D11Device device, out D3D11DeviceContext context)
	{
		device  = null!;
		context = null!;

		loader.LoadFunction("d3d11", "D3D11CreateDevice", out PfnD3D11CreateDevice construct);
		if (construct(adapter?.Self ?? IntPtr.Zero,
		              (uint)driverType,
		              IntPtr.Zero,
		              (uint)D3D11CreateDeviceFlags.Singlethreaded,
		              IntPtr.Zero,
		              0u,
		              (uint)D3D11SdkVersion.Current,
		              out var devicePointer,
		              out var featureLevel,
		              out var contextPointer).Success)
		{
			device  = new D3D11Device(devicePointer, (Direct3DFeatureLevel) featureLevel);
			context = new D3D11DeviceContext(contextPointer);
			return true;
		}

		return false;
	}
}
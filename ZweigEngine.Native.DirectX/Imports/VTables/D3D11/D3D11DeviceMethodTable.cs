using System.Runtime.InteropServices;

namespace ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

[Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140")]
[StructLayout(LayoutKind.Sequential)]
internal struct D3D11DeviceMethodTable
{
	public UnknownMethodTable Super;
	public IntPtr             CreateBuffer;
	public IntPtr             CreateTexture1D;
	public IntPtr             CreateTexture2D;
	public IntPtr             CreateTexture3D;
	public IntPtr             CreateShaderResourceView;
	public IntPtr             CreateUnorderedAccessView;
	public IntPtr             CreateRenderTargetView;
	public IntPtr             CreateDepthStencilView;
	public IntPtr             CreateInputLayout;
	public IntPtr             CreateVertexShader;
	public IntPtr             CreateGeometryShader;
	public IntPtr             CreateGeometryShaderWithStreamOutput;
	public IntPtr             CreatePixelShader;
	public IntPtr             CreateHullShader;
	public IntPtr             CreateDomainShader;
	public IntPtr             CreateComputeShader;
	public IntPtr             CreateClassLinkage;
	public IntPtr             CreateBlendState;
	public IntPtr             CreateDepthStencilState;
	public IntPtr             CreateRasterizerState;
	public IntPtr             CreateSamplerState;
	public IntPtr             CreateQuery;
	public IntPtr             CreatePredicate;
	public IntPtr             CreateCounter;
	public IntPtr             CreateDeferredContext;
	public IntPtr             OpenSharedResource;
	public IntPtr             CheckFormatSupport;
	public IntPtr             CheckMultisampleQualityLevels;
	public IntPtr             CheckCounterInfo;
	public IntPtr             CheckCounter;
	public IntPtr             CheckFeatureSupport;
	public IntPtr             GetPrivateData;
	public IntPtr             SetPrivateData;
	public IntPtr             SetPrivateDataInterface;
	public IntPtr             GetFeatureLevel;
	public IntPtr             GetCreationFlags;
	public IntPtr             GetDeviceRemovedReason;
	public IntPtr             GetImmediateContext;
	public IntPtr             SetExceptionMode;
	public IntPtr             GetExceptionMode;
}
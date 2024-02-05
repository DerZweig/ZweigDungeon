using System.Runtime.InteropServices;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.DirectX.Imports.D3D11.Structures;
using ZweigEngine.Native.DirectX.Imports.VTables.D3D11;

namespace ZweigEngine.Native.DirectX.Imports.D3D11;

internal sealed class D3D11DeviceContext : DXObject
{
	private delegate void PfnClearStateDelegate(IntPtr self);
	private delegate void PfnFlushDelegate(IntPtr self);
	private delegate void PfnRasterSetViewports(IntPtr self, uint count, IntPtr viewports);
	private delegate void PfnOutputSetRenderTargets(IntPtr self, uint count, IntPtr targets, IntPtr depthTarget);
	private delegate void PfnClearRenderTargetView(IntPtr self, IntPtr renderTarget, IntPtr colorRGBA);

	private readonly PfnClearStateDelegate     m_clearState;
	private readonly PfnFlushDelegate          m_flush;
	private readonly PfnClearRenderTargetView  m_clearRenderTargetView;
	private readonly PfnRasterSetViewports     m_rasterSetViewports;
	private readonly PfnOutputSetRenderTargets m_outputSetRenderTargets;

	internal D3D11DeviceContext(IntPtr pointer) : base(pointer)
	{
		LoadMethod((in D3D11DeviceContextMethodTable table) => table.ClearState, out m_clearState);
		LoadMethod((in D3D11DeviceContextMethodTable table) => table.Flush, out m_flush);
		LoadMethod((in D3D11DeviceContextMethodTable table) => table.ClearRenderTargetView, out m_clearRenderTargetView);
		LoadMethod((in D3D11DeviceContextMethodTable table) => table.RSSetViewports, out m_rasterSetViewports);
		LoadMethod((in D3D11DeviceContextMethodTable table) => table.OMSetRenderTargets, out m_outputSetRenderTargets);
	}
	
	public void SetViewport(in D3D11Viewport viewport)
	{
		using var pinned = new PinnedObject<D3D11Viewport>(viewport, GCHandleType.Pinned);
		m_rasterSetViewports(Self, 1u, pinned.GetAddress());
	}


	public void SetViewports(D3D11Viewport[] viewports)
	{
		using var pinned = new PinnedObject<D3D11Viewport[]>(viewports, GCHandleType.Pinned);
		m_rasterSetViewports(Self, (uint)viewports.Length, pinned.GetAddress());
	}

	public void SetRenderTarget(D3D11RenderTargetView? renderTarget)
	{
		if (renderTarget != null)
		{
			SetRenderTargets(new[] { renderTarget });
		}
		else
		{
			SetRenderTargets(Array.Empty<D3D11RenderTargetView>());
		}
	}
	
	public void SetRenderTargets(D3D11RenderTargetView[] renderTargets)
	{
		if (renderTargets.Any())
		{
			var       pointers = renderTargets.Select(x => x.Self).ToArray();
			using var pinned   = new PinnedObject<IntPtr[]>(pointers, GCHandleType.Pinned);
			m_outputSetRenderTargets(Self, (uint)pointers.Length, pinned.GetAddress(), IntPtr.Zero);
		}
		else
		{
			m_outputSetRenderTargets(Self, 0u, IntPtr.Zero, IntPtr.Zero);
		}
	}

	public void ClearState()
	{
		m_clearState(Self);
	}

	public void Flush()
	{
		m_flush(Self);
	}

	public void ClearRenderTargetView(D3D11RenderTargetView renderTarget, float red, float green, float blue, float alpha)
	{
		var       colorFloat = new float[] { red, green, blue, alpha };
		using var pinned     = new PinnedObject<float[]>(colorFloat, GCHandleType.Pinned);
		m_clearRenderTargetView(Self, renderTarget.Self, pinned.GetAddress());
	}
}
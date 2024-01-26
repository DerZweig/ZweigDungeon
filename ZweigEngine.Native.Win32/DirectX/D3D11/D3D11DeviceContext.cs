namespace ZweigEngine.Native.Win32.DirectX.D3D11;

internal sealed class D3D11DeviceContext : DirectXObject
{
	private delegate void PfnClearStateDelegate(IntPtr self);
	private delegate void PfnFlushDelegate(IntPtr self);

	private readonly PfnClearStateDelegate m_clearState;
	private readonly PfnFlushDelegate      m_flush;

	internal D3D11DeviceContext(IntPtr pointer) : base(pointer)
	{
		LoadMethod(110, out m_clearState);
		LoadMethod(111, out m_flush);
	}

	public void ClearState()
	{
		m_clearState(Self);
	}

	public void Flush()
	{
		m_flush(Self);
	}
}
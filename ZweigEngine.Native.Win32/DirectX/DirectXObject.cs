﻿using System.Runtime.InteropServices;

namespace ZweigEngine.Native.Win32.DirectX;

internal abstract class DirectXObject : IDisposable
{
	private IntPtr m_pointer;

	protected DirectXObject(IntPtr pointer)
	{
		m_pointer = pointer;
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_pointer != IntPtr.Zero)
		{
			Marshal.Release(m_pointer);
			m_pointer = IntPtr.Zero;
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~DirectXObject()
	{
		ReleaseUnmanagedResources();
	}

	public IntPtr Self => m_pointer;

	protected void LoadMethod<TDelegate>(uint index, out TDelegate method) where TDelegate : Delegate
	{
		var offset  = Marshal.SizeOf<IntPtr>() * new IntPtr(index);
		var slot    = Marshal.PtrToStructure<IntPtr>(m_pointer) + offset;
		var address = Marshal.PtrToStructure<IntPtr>(slot);
		method = Marshal.GetDelegateForFunctionPointer<TDelegate>(address);
	}
}
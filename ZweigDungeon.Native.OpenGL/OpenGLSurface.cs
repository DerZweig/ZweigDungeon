using System.Runtime.InteropServices;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Utility.Interop;

namespace ZweigDungeon.Native.OpenGL;

internal class OpenGLSurface : IVideoSurface
{
	private readonly PinnedObject<VideoColor[]> m_pinned;

	public OpenGLSurface(OpenGLContext context, ushort width, ushort height)
	{
		Context  = context;
		Width    = width;
		Height   = height;
		Data     = new VideoColor[width * height];
		m_pinned = new PinnedObject<VideoColor[]>(Data, GCHandleType.Pinned);
	}

	private void ReleaseUnmanagedResources()
	{
		try
		{
			Context?.DestroySurface(this);
		}
		finally
		{
			m_pinned.Dispose();
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLSurface()
	{
		ReleaseUnmanagedResources();
	}

	public   ushort         Width   { get; }
	public   ushort         Height  { get; }
	internal VideoColor[]   Data    { get; }
	internal OpenGLContext? Context { get; set; }
	internal IntPtr         Address => m_pinned.GetAddress();
}
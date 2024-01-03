using System.Runtime.InteropServices;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Utility.Interop;

namespace ZweigEngine.Native.OpenGL;

internal class OpenGLImage : IVideoImage
{
	private readonly VideoColor[]               m_data;
	private readonly PinnedObject<VideoColor[]> m_pinned;

	private OpenGLContext? m_context;

	public OpenGLImage(OpenGLContext context, ushort width, ushort height)
	{
		m_context = context;
		Width     = width;
		Height    = height;
		m_data    = new VideoColor[width * height];
		m_pinned  = new PinnedObject<VideoColor[]>(m_data, GCHandleType.Pinned);
	}

	private void ReleaseUnmanagedResources()
	{
		try
		{
			m_context?.ReleaseTexture(this);
			m_context = null;
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

	~OpenGLImage()
	{
		ReleaseUnmanagedResources();
	}

	public ushort Width  { get; }
	public ushort Height { get; }

	internal IntPtr Address => m_pinned.GetAddress();

	public void Map(Action<VideoColor[]> mapper)
	{
		if (m_context != null)
		{
			m_context.BindTexture(null);
			mapper(m_data);
		}

		m_context?.UploadTexture(this);
	}

	public void Blit(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags)
	{
		if (m_context != null)
		{
			m_context.BindTexture(this);
			m_context.DrawSurface(dstRegion, srcRegion, tintColor, blitFlags);
		}
	}
}
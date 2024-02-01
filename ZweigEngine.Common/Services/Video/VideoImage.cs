using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Interfaces;
using ZweigEngine.Common.Services.Video.Structures;
using ZweigEngine.Common.Utility.Interop;

namespace ZweigEngine.Common.Services.Video;

internal class VideoImage : IVideoImage
{
	private readonly PinnedObject<VideoColor[]> m_pinned;

	private VideoContext? m_context;

	public VideoImage(VideoContext context, ushort width, ushort height)
	{
		m_context = context;
		Width     = width;
		Height    = height;
		Data      = new VideoColor[width * height];
		m_pinned  = new PinnedObject<VideoColor[]>(Data, GCHandleType.Pinned);
	}

	private void ReleaseUnmanagedResources()
	{
		try
		{
			m_context?.DestroyImage(this);
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

	~VideoImage()
	{
		ReleaseUnmanagedResources();
	}

	public   ushort       Width   { get; }
	public   ushort       Height  { get; }
	internal VideoColor[] Data    { get; }
	internal IntPtr       Address => m_pinned.GetAddress();

	public void Map(Action<VideoColor[]> mapper)
	{
		m_context?.Map(this, mapper);
	}

	public void Blit(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags)
	{
		m_context?.DrawImage(this, dstRegion, srcRegion, tintColor, blitFlags);
	}
}
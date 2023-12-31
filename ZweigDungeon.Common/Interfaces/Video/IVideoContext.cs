namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoContext
{
	void SetBlendMode(VideoBlendMode mode);
	void CreateSurface(ushort width, ushort height, out IVideoImage image);
	void DestroySurface(IVideoImage image);
	void MapSurfaceData(IVideoImage image, Action<VideoColor[]> mapper);
	void DrawSurface(IVideoImage image, in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags);
}
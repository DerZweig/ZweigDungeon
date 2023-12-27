namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoContext
{
	void BeginFrame(int viewportWidth, int viewportHeight);
	void FinishFrame();

	void CreateSurface(ushort width, ushort height, out IVideoSurface surface);
	void DestroySurface(IVideoSurface surface);
	void MapSurfaceData(IVideoSurface surface, Action<VideoColor[]> mapper);
	void DrawSurface(IVideoSurface surface, in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoFlags flags);
}
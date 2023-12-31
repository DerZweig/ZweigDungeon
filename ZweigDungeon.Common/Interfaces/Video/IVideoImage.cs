namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoImage : IDisposable
{
	ushort Width  { get; }
	ushort Height { get; }
	
	void Map(Action<VideoColor[]> mapper);
	void Blit(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags);
}
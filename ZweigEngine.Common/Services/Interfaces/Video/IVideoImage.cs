using ZweigEngine.Common.Services.Interfaces.Video.Constant;
using ZweigEngine.Common.Services.Interfaces.Video.Structures;

namespace ZweigEngine.Common.Services.Interfaces.Video;

public interface IVideoImage : IDisposable
{
	ushort Width  { get; }
	ushort Height { get; }
	
	void Map(Action<VideoColor[]> mapper);
	void Blit(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags);
}
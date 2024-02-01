using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Services.Video.Interfaces;

public interface IVideoImage : IDisposable
{
	ushort Width  { get; }
	ushort Height { get; }
	
	void Map(Action<VideoColor[]> mapper);
	void Blit(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags);
}
using ZweigEngine.Common.Services.Interfaces.Video.Constant;

namespace ZweigEngine.Common.Services.Interfaces.Video;

public interface IVideoContext
{
	void SetBlendMode(VideoBlendMode mode);
	void CreateSurface(ushort width, ushort height, out IVideoImage image);
}
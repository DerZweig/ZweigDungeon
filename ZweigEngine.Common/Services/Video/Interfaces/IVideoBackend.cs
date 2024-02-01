using ZweigEngine.Common.Services.Video.Constant;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Services.Video.Interfaces;

public interface IVideoBackend
{
	void BeginScene(in VideoRect viewport);
	void FinishScene();
	
	void SetBlendMode(VideoBlendMode mode);
	void FlushPending();
	
	void CreateImage(ushort width, ushort height, IntPtr data, out uint name);
	void DestroyImage(uint name);
	void BindImage(uint? name);
	void UploadImage(uint name, ushort width, ushort height, IntPtr data);
	void DrawImage(in VideoRect dstRegion, in VideoRect srcRegion, in VideoColor tintColor, VideoBlitFlags blitFlags);
}
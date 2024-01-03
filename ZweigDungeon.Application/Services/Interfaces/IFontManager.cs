using ZweigDungeon.Application.Services.Constants;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IFontManager
{
	void Layout(FontSize size, int viewportWidth, string text, out string result);
	void Draw(FontSize size, string text, int left, int top, VideoRect clip, VideoColor color);
}
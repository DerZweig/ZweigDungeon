using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuRenderer
{
	void Draw(in VideoRect viewport);
}
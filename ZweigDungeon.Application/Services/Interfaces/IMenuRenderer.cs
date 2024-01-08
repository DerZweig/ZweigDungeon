using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuRenderer
{
	void Draw(Menu menu, in VideoRect viewport);
}
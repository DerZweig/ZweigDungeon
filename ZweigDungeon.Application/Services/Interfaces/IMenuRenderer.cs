using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuRenderer
{
	void Draw(MenuDefinition menu, in VideoRect viewport);
}
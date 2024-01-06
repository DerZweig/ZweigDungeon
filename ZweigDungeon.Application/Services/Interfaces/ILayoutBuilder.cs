using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface ILayoutBuilder
{
	void UpdateLayout(MenuDefinition menu, in VideoRect viewport);
}
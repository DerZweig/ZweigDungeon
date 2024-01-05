using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuManager
{
	public Task<MenuDefinition> LoadMenu(string name);
}
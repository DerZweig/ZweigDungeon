using ZweigDungeon.Application.Entities.Assets;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuRepository
{
	public Task<Menu> LoadMenu(string name);
}
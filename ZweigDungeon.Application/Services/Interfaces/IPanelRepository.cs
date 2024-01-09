using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Implementation;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IPanelRepository
{
	Task<Panel> LoadPanel(string path, string name);
}
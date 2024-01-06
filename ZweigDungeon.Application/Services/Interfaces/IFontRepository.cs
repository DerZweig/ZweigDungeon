using ZweigDungeon.Application.Entities.Assets;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IFontRepository
{
	Task<FontDefinition> LoadFont(string name);
}
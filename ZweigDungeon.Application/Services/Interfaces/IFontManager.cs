using ZweigDungeon.Application.Entities.Assets;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IFontManager
{
	Task<FontDefinition> LoadFont(string name);
}
using ZweigDungeon.Application.Entities.Assets;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IFontRepository
{
	Task<Font> LoadFont(string path);
}
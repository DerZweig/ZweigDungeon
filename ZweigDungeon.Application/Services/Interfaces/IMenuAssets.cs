using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigEngine.Common.Assets.Font;
using ZweigEngine.Common.Assets.Image;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuAssets
{
	FontAsset  GetFontDefinition(FontSize font);
	ImageAsset GetFontImage(FontSize font);
}
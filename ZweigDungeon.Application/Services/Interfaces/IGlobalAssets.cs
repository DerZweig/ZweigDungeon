using ZweigDungeon.Application.Gui.Constants;
using ZweigEngine.Common.Assets.Font;
using ZweigEngine.Common.Assets.Image;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IGlobalAssets
{
	bool       IsLoaded();
	FontAsset  GetFontDefinition(FontSize font);

	ImageAsset GetSolidColorImage();
	ImageAsset GetFontImage(FontSize font);
	ImageAsset GetCharacterImage();
}
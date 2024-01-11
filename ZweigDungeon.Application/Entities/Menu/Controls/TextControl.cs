using ZweigEngine.Common.Assets.Font;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class TextControl : BasicControl
{
	public FontAsset Font { get; set; } = FontAsset.Empty;
	public string    Text { get; set; } = string.Empty;
}
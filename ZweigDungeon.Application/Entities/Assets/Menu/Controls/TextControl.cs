using ZweigDungeon.Application.Entities.Assets.Menu.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Menu.Controls;

public class TextControl : BasicControl
{
	public FontSize FontSize   { get; set; }
	public string   Text       { get; set; } = string.Empty;
	public string   LayoutText { get; set; } = string.Empty;
}
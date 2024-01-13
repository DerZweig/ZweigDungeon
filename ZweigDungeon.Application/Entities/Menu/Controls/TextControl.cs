using ZweigDungeon.Application.Entities.Menu.Constants;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class TextControl : BasicControl
{
	public FontSize Font { get; set; } = FontSize.Medium;
	public string   Text { get; set; } = string.Empty;
}
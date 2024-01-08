using ZweigDungeon.Application.Entities.Assets.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Controls;

public class TextControl : BasicControl
{
	public FontSize FontSize   { get; set; }
	public string   Text       { get; set; } = string.Empty;
}
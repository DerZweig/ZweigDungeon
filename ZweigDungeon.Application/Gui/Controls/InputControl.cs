using ZweigDungeon.Application.Gui.Constants;

namespace ZweigDungeon.Application.Gui.Controls;

public class InputControl : BasicControl
{
	public FontSize TextFont      { get; set; } = FontSize.Medium;
	public string   CurrentText   { get; set; } = string.Empty;
	public int      MaximumLength { get; set; } = 32;
}
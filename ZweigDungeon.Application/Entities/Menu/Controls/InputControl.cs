using ZweigEngine.Common.Assets.Font;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class InputControl : BasicControl
{
	public FontAsset Font          { get; set; } = FontAsset.Empty;
	public string    CurrentText   { get; set; } = string.Empty;
	public int       MaximumLength { get; set; } = 32;
}
namespace ZweigDungeon.Application.Gui;

public abstract class BasicMenu : BasicPanel
{
	public int? MinimumWidth  { get; set; }
	public int? MinimumHeight { get; set; }
	public int? MaximumWidth  { get; set; }
	public int? MaximumHeight { get; set; }
}
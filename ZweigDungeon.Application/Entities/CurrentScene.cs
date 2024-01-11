using ZweigDungeon.Application.Entities.Menu.Panels;

namespace ZweigDungeon.Application.Entities;

public class CurrentScene
{
	public BasicPanels? MessagePanel { get; set; }
	public BasicPanels? MenuPanel    { get; set; }
	public BasicPanels? LeftPanel    { get; set; }
	public BasicPanels? RightPanel   { get; set; }
}
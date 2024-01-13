using ZweigDungeon.Application.Entities.Menu;

namespace ZweigDungeon.Application.Entities;

public class CurrentScene
{
	public ControlPanel? MessagePanel { get; set; }
	public ControlPanel? MenuPanel    { get; set; }
	public ControlPanel? LeftPanel    { get; set; }
	public ControlPanel? RightPanel   { get; set; }
}
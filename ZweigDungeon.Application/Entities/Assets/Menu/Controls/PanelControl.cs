using ZweigDungeon.Application.Entities.Assets.Menu.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Menu.Controls;

public class PanelControl : BasicControl
{
	public PanelControl()
	{
		Children = new List<BasicControl>();
	}

	public PanelLayout        Layout   { get; set; }
	public List<BasicControl> Children { get; }
}
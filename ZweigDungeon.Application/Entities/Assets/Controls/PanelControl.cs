using ZweigDungeon.Application.Entities.Assets.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Controls;

public class PanelControl : BasicControl
{
	public PanelControl()
	{
		Children = new List<BasicControl>();
	}

	public PanelLayout        Layout   { get; set; }
	public List<BasicControl> Children { get; }
}
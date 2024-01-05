using ZweigDungeon.Application.Entities.Assets.Menu.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Menu.Controls;

public class ButtonControl : BasicControl
{
	public ButtonControl()
	{
		Children = new List<BasicControl>();
	}

	public List<BasicControl> Children { get; }
}
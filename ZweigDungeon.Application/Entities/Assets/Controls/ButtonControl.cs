namespace ZweigDungeon.Application.Entities.Assets.Controls;

public class ButtonControl : BasicControl
{
	public ButtonControl()
	{
		Children = new List<BasicControl>();
	}

	public List<BasicControl> Children { get; }
}
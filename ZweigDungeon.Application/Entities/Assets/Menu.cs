using ZweigDungeon.Application.Entities.Assets.Controls;

namespace ZweigDungeon.Application.Entities.Assets;

public class Menu
{
	public Menu()
	{
		Panel = new PanelControl();
	}

	public PanelControl Panel { get; }
}
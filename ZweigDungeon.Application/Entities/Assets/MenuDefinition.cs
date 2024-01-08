using ZweigDungeon.Application.Entities.Assets.Menu.Controls;

namespace ZweigDungeon.Application.Entities.Assets;

public class MenuDefinition
{
	public MenuDefinition()
	{
		Panel = new PanelControl();
	}

	public PanelControl Panel { get; }
}
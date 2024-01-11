using System.Collections.Generic;
using ZweigDungeon.Application.Entities.Menu.Controls;
using ZweigEngine.Common.Assets.Tiles;

namespace ZweigDungeon.Application.Entities.Menu.Panels;

public abstract class BasicPanels
{
	private List<BasicControl> m_children;

	protected BasicPanels()
	{
		m_children = new ();
	}
	
	public TileSheetTile             Background { get; set; } = new();
	public IEnumerable<BasicControl> Children   => m_children;
}
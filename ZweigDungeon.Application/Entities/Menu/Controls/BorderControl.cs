using ZweigEngine.Common.Assets.Tiles;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class BorderControl : BasicControl
{
	public TileSheetTile Background { get; set; } = new();
}
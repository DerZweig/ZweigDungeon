using ZweigDungeon.Application.Entities.Menu.Controls;
using ZweigEngine.Common.Assets.Tiles;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Menu;

public class ControlPanel
{
	public VideoRect          LayoutRecct { get; set; }
	public TileSheetTile      Background  { get; set; } = new();
	public List<BasicControl> Children    { get; }      = new();
}
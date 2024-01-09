using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets;

public class PanelTile
{
	public VideoRect ImageRect    { get; set; }
	public int       BorderLeft   { get; set; }
	public int       BorderTop    { get; set; }
	public int       BorderRight  { get; set; }
	public int       BorderBottom { get; set; }
}
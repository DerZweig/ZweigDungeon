using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets;

public class Panel
{
	public string    ImageName    { get; set; } = string.Empty;
	public VideoRect ImageRect    { get; set; }
	public int       BorderLeft   { get; set; }
	public int       BorderTop    { get; set; }
	public int       BorderRight  { get; set; }
	public int       BorderBottom { get; set; }
}
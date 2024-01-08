using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets;

public class FontChar
{
	public VideoRect ImageRect  { get; init; }
	public int       OffsetLeft { get; init; }
	public int       OffsetTop  { get; init; }
	public int       Advance    { get; init; }
}
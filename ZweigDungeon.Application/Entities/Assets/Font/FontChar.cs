using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets.Font;

public class FontChar
{
	public VideoRect ImageRect  { get; init; }
	public int       OffsetLeft { get; init; }
	public int       OffsetTop  { get; init; }
	public int       Advance    { get; init; }
}
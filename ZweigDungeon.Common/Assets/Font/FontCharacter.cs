using ZweigDungeon.Common.Interfaces.Video;

namespace ZweigDungeon.Common.Assets.Font;

public struct FontCharacter
{
	public char      Character  { get; init; }
	public VideoRect Rect       { get; init; }
	public int       OffsetLeft { get; init; }
	public int       OffsetTop  { get; init; }
	public int       Advance    { get; init; }
}
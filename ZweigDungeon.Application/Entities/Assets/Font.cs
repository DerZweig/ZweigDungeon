namespace ZweigDungeon.Application.Entities.Assets;

public class Font
{
	public IReadOnlyDictionary<char, FontChar>   Chars      { get; init; } = null!;
	public IReadOnlyDictionary<FontKerning, int> Kernings   { get; init; } = null!;
	public int                                   LineHeight { get; init; } = 0;
}
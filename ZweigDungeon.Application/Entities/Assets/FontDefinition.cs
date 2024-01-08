using ZweigDungeon.Application.Entities.Assets.Font;

namespace ZweigDungeon.Application.Entities.Assets;

public class FontDefinition
{
	public IReadOnlyDictionary<char, FontChar>   Chars      { get; init; } = null!;
	public IReadOnlyDictionary<FontKerning, int> Kernings   { get; init; } = null!;
	public int                                   LineHeight { get; init; } = 0;
}
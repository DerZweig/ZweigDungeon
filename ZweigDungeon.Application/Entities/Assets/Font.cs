namespace ZweigDungeon.Application.Entities.Assets;

public class Font
{
	private static readonly IReadOnlyDictionary<char, FontChar>   g_emptyChars;
	private static readonly IReadOnlyDictionary<FontKerning, int> g_emptyKernings;
	private static readonly Font                                  g_emptyFont;

	static Font()
	{
		g_emptyChars    = new Dictionary<char, FontChar>();
		g_emptyKernings = new Dictionary<FontKerning, int>();
		g_emptyFont     = new Font();
	}

	public static Font Empty => g_emptyFont;
	
	public IReadOnlyDictionary<char, FontChar>   Chars      { get; init; } = g_emptyChars;
	public IReadOnlyDictionary<FontKerning, int> Kernings   { get; init; } = g_emptyKernings;
	public int                                   LineHeight { get; init; } = 0;
}
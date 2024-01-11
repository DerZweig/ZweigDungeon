namespace ZweigEngine.Common.Assets.Font;

public sealed class FontAsset
{
	private static readonly IReadOnlyDictionary<char, FontCharacter> g_emptyChars;
	private static readonly IReadOnlyDictionary<FontKerning, int>    g_emptyKernings;

	static FontAsset()
	{
		g_emptyChars    = new Dictionary<char, FontCharacter>();
		g_emptyKernings = new Dictionary<FontKerning, int>();
		Empty           = new FontAsset();
	}

	public static FontAsset Empty { get; }

	public IReadOnlyDictionary<char, FontCharacter> Chars      { get; init; } = g_emptyChars;
	public IReadOnlyDictionary<FontKerning, int>    Kernings   { get; init; } = g_emptyKernings;
	public int                                      LineHeight { get; init; } = 0;
}
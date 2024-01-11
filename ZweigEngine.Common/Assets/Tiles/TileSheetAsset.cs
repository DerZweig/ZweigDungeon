namespace ZweigEngine.Common.Assets.Tiles;

public class TileSheetAsset
{
	private static readonly IReadOnlyDictionary<string, TileSheetTile> g_emptyTiles;

	static TileSheetAsset()
	{
		g_emptyTiles = new Dictionary<string, TileSheetTile>();
		Empty        = new TileSheetAsset();
	}

	public static TileSheetAsset Empty { get; }

	public IReadOnlyDictionary<string, TileSheetTile> Tiles { get; init; } = g_emptyTiles;
}
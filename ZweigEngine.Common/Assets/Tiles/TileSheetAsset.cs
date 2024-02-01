using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigEngine.Common.Assets.Tiles;

public class TileSheetAsset
{
	private static readonly IReadOnlyDictionary<string, VideoRect> g_emptyTiles;

	static TileSheetAsset()
	{
		g_emptyTiles = new Dictionary<string, VideoRect>();
		Empty        = new TileSheetAsset();
	}

	public static TileSheetAsset Empty { get; }

	public IReadOnlyDictionary<string, VideoRect> Tiles { get; init; } = g_emptyTiles;
}
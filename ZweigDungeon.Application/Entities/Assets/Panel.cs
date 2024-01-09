namespace ZweigDungeon.Application.Entities.Assets;

public class Panel
{
	private static readonly IReadOnlyDictionary<string, PanelTile> g_emptyTiles;

	static Panel()
	{
		g_emptyTiles = new Dictionary<string, PanelTile>();
	}

	public IReadOnlyDictionary<string, PanelTile> Tiles { get; set; } = g_emptyTiles;
}
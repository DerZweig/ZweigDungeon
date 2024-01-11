using ZweigEngine.Common.Assets.Tiles;

namespace ZweigEngine.Common.Services.Repositories;

public class TileSheetRepository : BasicAsyncRepository<TileSheetAsset>
{
	protected override Task<TileSheetAsset> LoadContents(string path, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
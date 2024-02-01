using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Repositories;
using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigDungeon.Application.Services.Implementation;

public class MapController : IMapController
{
	private const int TILE_IMAGE_WIDTH      = 22;
	private const int TILE_IMAGE_HEIGHT     = 22;
	private const int TILE_COUNT_HORIZONTAL = 24;
	private const int TILE_COUNT_VERTICAL   = 16;

	private readonly IGlobalAssets     m_assets;
	private readonly TextureRepository m_textures;

	private int m_viewportLeft;
	private int m_viewportTop;
	private int m_tileWidth;
	private int m_tileHeight;

	public MapController(IGlobalAssets assets, TextureRepository textures)
	{
		m_assets   = assets;
		m_textures = textures;
	}

	public void Update(in VideoRect viewport)
	{
		//this does not work correctly
		var width     = viewport.Width / TILE_COUNT_HORIZONTAL;
		var height    = viewport.Height / TILE_COUNT_VERTICAL;
		var mulWidth  = Math.Max(width / TILE_IMAGE_WIDTH, 1);
		var mulHeight = Math.Max(height / TILE_IMAGE_HEIGHT, 1);
		var mulBest   = Math.Max(mulWidth, mulHeight);

		var viewWidth  = m_tileWidth * TILE_COUNT_HORIZONTAL;
		var viewHeight = m_tileHeight * TILE_COUNT_VERTICAL;

		m_tileWidth    = mulBest * TILE_IMAGE_WIDTH;
		m_tileHeight   = mulBest * TILE_IMAGE_HEIGHT;
		m_viewportLeft = viewport.Width / 2 - viewWidth / 2;
		m_viewportTop  = viewport.Height / 2 - viewHeight / 2;
	}

	public void Display(in VideoRect viewport)
	{
		/*
		var greyColor = new VideoColor { Red  = 128, Green          = 128, Blue = 128, Alpha = 255 };
		var srcRect   = new VideoRect { Width = 8, Height           = 8 };
		var dstRect   = new VideoRect { Width = m_tileWidth, Height = m_tileHeight };

		m_textures.BindOrIgnore(m_assets.GetSolidColorImage(), texture =>
		{
			for (var y = 0; y < TILE_COUNT_VERTICAL; ++y)
			{
				dstRect = dstRect with { Top = m_tileHeight * y + m_viewportTop };

				for (var x = y % 2; x < TILE_COUNT_HORIZONTAL; x += 2)
				{
					dstRect = dstRect with { Left = m_tileWidth * x + m_viewportLeft };
					texture.Blit(dstRect, srcRect, greyColor, VideoBlitFlags.None);
				}
			}
		});*/
	}
}
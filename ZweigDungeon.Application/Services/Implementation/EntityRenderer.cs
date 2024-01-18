using ZweigDungeon.Application.Entities;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Repositories;

namespace ZweigDungeon.Application.Services.Implementation;

public class EntityRenderer : IEntityRenderer
{
	private readonly CurrentScene      m_scene;
	private readonly IEntityAssets     m_assets;
	private readonly TextureRepository m_textures;
	private          DateTime          m_lastAnimTime;
	private          int               m_animFrame;

	public EntityRenderer(CurrentScene scene, IEntityAssets assets, TextureRepository textures)
	{
		m_scene        = scene;
		m_assets       = assets;
		m_textures     = textures;
		m_lastAnimTime = DateTime.Now;
	}

	public void Draw(in VideoRect viewport)
	{
		var animTime    = DateTime.Now;
		var animElapsed = animTime - m_lastAnimTime;

		m_animFrame = (int)(animElapsed.TotalSeconds * 6);

		var actorImg = m_assets.GetActorImage();

		m_textures.BindOrIgnore(actorImg, texture =>
		{
			var tileAnim = (m_animFrame % 4) switch
			               {
				               1 => 1,
				               2 => 0,
				               3 => 2,
				               _ => 0
			               };

			var weaponColor = new VideoColor { Red = 255, Green = 255, Blue  = 255, Alpha = 255 };
			var skinColor   = new VideoColor { Red = 255, Green = 166, Blue  = 128, Alpha = 255 };
			var hairColor   = new VideoColor { Red = 255, Green = 55, Blue   = 55, Alpha  = 255 };
			var shirtColor  = new VideoColor { Red = 128, Green = 196, Blue  = 196, Alpha = 255 };
			var bootColor   = new VideoColor { Red = 255, Green = 55, Blue   = 0, Alpha   = 255 };
			var dstRect     = new VideoRect { Left = 100, Top   = 100, Width = 88, Height = 88 };

			DrawActor(texture, 0, 1, dstRect, skinColor);
			DrawActor(texture, 12, 2, dstRect, weaponColor);

			dstRect = dstRect with
			{
				Left = 200
			};

			DrawActor(texture, tileAnim, 1, dstRect, skinColor);
			DrawActor(texture, tileAnim + 8, 5, dstRect, bootColor);
			DrawActor(texture, tileAnim, 5, dstRect, shirtColor);
			DrawActor(texture, 5, 1, dstRect, hairColor);

			dstRect = dstRect with
			{
				Left = 300
			};

			shirtColor = shirtColor with
			{
				Red = 168,
				Green = 168,
				Blue = 54
			};

			DrawActor(texture, tileAnim, 1, dstRect, skinColor);
			DrawActor(texture, tileAnim + 8, 5, dstRect, bootColor);
			DrawActor(texture, tileAnim + 4, 9, dstRect, shirtColor);
			DrawActor(texture, tileAnim, 5, dstRect, shirtColor);
			DrawActor(texture, 7, 1, dstRect, hairColor);
			
			dstRect = dstRect with
			{
				Left = 400
			};

			hairColor = hairColor with
			{
				Red = 122,
				Green = 72
			};

			shirtColor = shirtColor with
			{
				Red = 255,
				Green = 54,
				Blue = 128
			};

			DrawActor(texture, tileAnim, 1, dstRect, skinColor);
			DrawActor(texture, tileAnim + 8, 5, dstRect, bootColor);
			DrawActor(texture, tileAnim + 4, 5, dstRect, shirtColor);
			DrawActor(texture, tileAnim, 5, dstRect, shirtColor);
			DrawActor(texture, 8, 1, dstRect, hairColor);


			dstRect = dstRect with
			{
				Left = 500
			};

			DrawActor(texture, tileAnim, 3, dstRect, skinColor);
			DrawActor(texture, tileAnim + 8, 7, dstRect, bootColor);
			DrawActor(texture, tileAnim + 4, 11, dstRect, shirtColor);
			DrawActor(texture, tileAnim, 7, dstRect, shirtColor);
			DrawActor(texture, 9, 3, dstRect, hairColor);
		});
	}

	private void DrawActor(IVideoImage texture, int tileX, int tileY, in VideoRect dst, in VideoColor color)
	{
		const int tileHeight = 22;
		const int tileWidth  = 22;

		var src = new VideoRect
		{
			Left   = tileX * tileWidth,
			Top    = tileY * tileHeight,
			Width  = tileWidth,
			Height = tileHeight
		};

		texture.Blit(dst, src, color, VideoBlitFlags.None);
	}
}
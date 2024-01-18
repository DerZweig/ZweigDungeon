using ZweigDungeon.Application.Entities;
using ZweigDungeon.Application.Entities.Menu;
using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigDungeon.Application.Entities.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Assets.Font;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Repositories;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuRenderer : IMenuRenderer
{
	private readonly CurrentScene      m_scene;
	private readonly IMenuAssets       m_assets;
	private readonly TextureRepository m_textures;

	public MenuRenderer(CurrentScene scene, IMenuAssets assets, TextureRepository textures)
	{
		m_scene    = scene;
		m_assets   = assets;
		m_textures = textures;
	}

	public void Draw(in VideoRect viewport)
	{
		var halfWidth = viewport.Width / 2;
		var leftView  = viewport with { Width = halfWidth };
		var rightView = viewport with { Left = viewport.Left + halfWidth, Width = halfWidth };
		DrawPanel(m_scene.LeftPanel, leftView);
		DrawPanel(m_scene.RightPanel, rightView);
		DrawPanel(m_scene.MenuPanel, viewport);
		DrawPanel(m_scene.MessagePanel, viewport);
	}

	private void DrawPanel(ControlPanel? panel, in VideoRect clip)
	{
		if (panel == null)
		{
			return;
		}

		foreach (var control in panel.Children)
		{
			switch (control)
			{
				case BorderControl border:
					break;
				case InputControl input:
					break;
				case ButtonControl button:
					DrawString(button.Label, button.LabelFont, control.LayoutRect.Left, control.LayoutRect.Top, button.LabelColor, button.LayoutRect);
					break;
				case TextControl text:
					break;
				case ToggleControl toggle:
					break;
			}
		}
	}

	private int MeasureString(string text, FontSize font)
	{
		return 0;
	}

	private void DrawString(string text, FontSize font, int left, int top, VideoColor color, VideoRect clip)
	{
		var fontDef = m_assets.GetFontDefinition(font);
		var fontImg = m_assets.GetFontImage(font);
		m_textures.BindOrIgnore(fontImg, texture =>
		{
			using (var enumerator = text.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					var currentChar = enumerator.Current;
					while (enumerator.MoveNext())
					{
						var nextChar = enumerator.Current;
						var kerning  = new FontKerning(currentChar, nextChar);
						left += DrawCharacter(currentChar, fontDef, texture, left, top, clip, color);

						if (fontDef.Kernings.TryGetValue(kerning, out var amount))
						{
							left += amount;
						}

						currentChar = nextChar;
					}

					DrawCharacter(currentChar, fontDef, texture, left, top, clip, color);
				}
			}
		});
	}

	private static int DrawCharacter(char character, FontAsset font, IVideoImage texture, int left, int top, in VideoRect clip, in VideoColor color)
	{
		if (font.Chars.TryGetValue(character, out var charInfo) || font.Chars.TryGetValue(' ', out charInfo))
		{
			var src     = charInfo.ImageRect;
			var advance = charInfo.Advance + charInfo.OffsetLeft;

			left += charInfo.OffsetLeft;
			top  += charInfo.OffsetTop;

			if (clip.Left > left)
			{
				var offset = clip.Left - left;
				left      += offset;
				src.Left  += offset;
				src.Width -= offset;
			}

			if (clip.Top > top)
			{
				var offset = clip.Top - top;
				top        += offset;
				src.Left   += offset;
				src.Height -= offset;
			}

			var clipRight = clip.Left + clip.Width;
			var dstRight  = left + src.Width;
			if (clipRight < dstRight)
			{
				var offset = dstRight - clipRight;
				if (offset >= src.Width)
				{
					return advance;
				}

				src.Width -= offset;
			}

			var clipBottom = clip.Top + clip.Height;
			var dstBottom  = top + src.Height;
			if (clipBottom < dstBottom)
			{
				var offset = dstBottom - clipBottom;
				if (offset >= src.Height)
				{
					return advance;
				}

				src.Height -= offset;
			}

			var dst = src with
			{
				Left = left,
				Top = top
			};

			texture.Blit(dst, src, color, VideoBlitFlags.None);
			return advance;
		}

		return 0;
	}
}
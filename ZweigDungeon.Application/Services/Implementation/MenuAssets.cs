using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Assets.Font;
using ZweigEngine.Common.Assets.Image;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Repositories;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuAssets : IDisposable, IMenuAssets
{
	private readonly IPlatformSynchronization m_synchronization;
	private readonly IGlobalCancellation      m_cancellation;
	private readonly IPlatformWindow          m_window;
	private readonly FontRepository           m_fonts;
	private readonly TileSheetRepository      m_tiles;
	private readonly ImageRepository          m_images;
	private readonly TextureRepository        m_textures;
	private          FontAsset                m_smallFont;
	private          FontAsset                m_mediumFont;
	private          FontAsset                m_largeFont;
	private          ImageAsset               m_smallFontImage;
	private          ImageAsset               m_mediumFontImage;
	private          ImageAsset               m_largeFontImage;

	public MenuAssets(IPlatformSynchronization synchronization, IGlobalCancellation cancellation,
	                  IPlatformWindow window,
	                  FontRepository fonts, TileSheetRepository tiles, ImageRepository images, TextureRepository textures)
	{
		m_synchronization = synchronization;
		m_cancellation    = cancellation;
		m_window          = window;
		m_fonts           = fonts;
		m_tiles           = tiles;
		m_images          = images;
		m_textures        = textures;
		m_smallFont       = FontAsset.Empty;
		m_mediumFont      = FontAsset.Empty;
		m_largeFont       = FontAsset.Empty;
		m_smallFontImage  = ImageAsset.Empty;
		m_mediumFontImage = ImageAsset.Empty;
		m_largeFontImage  = ImageAsset.Empty;

		m_window.OnCreated += HandleWindowCreated;
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.OnCreated += HandleWindowCreated;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~MenuAssets()
	{
		ReleaseUnmanagedResources();
	}

	private void HandleWindowCreated(IPlatformWindow window)
	{
		var cancellationToken = m_cancellation.Token;
		m_synchronization.Invoke(async () =>
		{
			m_smallFont       = await m_fonts.LoadForGlobal("Gui/font_small", cancellationToken);
			m_mediumFont      = await m_fonts.LoadForGlobal("Gui/font_medium", cancellationToken);
			m_largeFont       = await m_fonts.LoadForGlobal("Gui/font_large", cancellationToken);
			m_smallFontImage  = await m_images.LoadForGlobal("Gui/font_small", cancellationToken);
			m_mediumFontImage = await m_images.LoadForGlobal("Gui/font_medium", cancellationToken);
			m_largeFontImage  = await m_images.LoadForGlobal("Gui/font_large", cancellationToken);
			await m_textures.LoadForGlobal(m_smallFontImage, cancellationToken);
			await m_textures.LoadForGlobal(m_mediumFontImage, cancellationToken);
			await m_textures.LoadForGlobal(m_largeFontImage, cancellationToken);
		}, cancellationToken);
	}

	public FontAsset GetFontDefinition(FontSize font)
	{
		switch (font)
		{
			case FontSize.Small:
				return m_smallFont;
			case FontSize.Medium:
				return m_mediumFont;
			case FontSize.Large:
				return m_largeFont;
			default:
				throw new UnhandledEnumException<FontSize>(font);
		}
	}

	public ImageAsset GetFontImage(FontSize font)
	{
		switch (font)
		{
			case FontSize.Small:
				return m_smallFontImage;
			case FontSize.Medium:
				return m_mediumFontImage;
			case FontSize.Large:
				return m_largeFontImage;
			default:
				throw new UnhandledEnumException<FontSize>(font);
		}
	}
}
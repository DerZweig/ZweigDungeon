using ZweigDungeon.Application.Gui;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Repositories;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuController : IDisposable, IMenuController
{
	private const int MINIMUM_STATUS_PANEL_HEIGHT = 100;
	private const int MINIMUM_SIDE_PANEL_WIDTH    = 300;
	private const int MAXIMUM_SIDE_PANEL_WIDTH    = 600;
	private const int MINIMUM_SIDE_PANEL_HEIGHT   = 350;
	private const int MAXIMUM_SIDE_PANEL_HEIGHT   = 800;
	private const int MINIMUM_AUTO_POPUP_WIDTH    = 300;

	private readonly IPlatformMouse    m_mouse;
	private readonly IPlatformKeyboard m_keyboard;
	private readonly IMenuVariables    m_menus;
	private readonly IGlobalAssets     m_assets;
	private readonly TextureRepository m_textures;
	private          VideoRect         m_dialogRect;
	private          VideoRect         m_menuRect;
	private          VideoRect         m_popupPanelRect;
	private          VideoRect         m_leftPanelRect;
	private          VideoRect         m_rightPanelRect;
	private          VideoRect         m_bottomPanelRect;
	private          BasicPanel?       m_focusedPanel;

	public MenuController(IPlatformMouse mouse, IPlatformKeyboard keyboard, IMenuVariables menus, IGlobalAssets assets, TextureRepository textures)
	{
		m_mouse    = mouse;
		m_keyboard = keyboard;
		m_menus    = menus;
		m_assets   = assets;
		m_textures = textures;

		m_mouse.OnMouseMoved            += HandleMouseMoved;
		m_mouse.OnMousePressed          += HandleMousePressed;
		m_mouse.OnMouseReleased         += HandleMouseReleased;
		m_mouse.OnMouseScrolledVertical += HandleMouseScrolled;
		m_keyboard.OnKeyPressed         += HandleKeyPressed;
		m_keyboard.OnKeyReleased        += HandleKeyReleased;
		m_keyboard.OnKeyTyped           += HandleKeyTyped;
	}

	private void ReleaseUnmanagedResources()
	{
		m_mouse.OnMouseMoved            -= HandleMouseMoved;
		m_mouse.OnMousePressed          -= HandleMousePressed;
		m_mouse.OnMouseReleased         -= HandleMouseReleased;
		m_mouse.OnMouseScrolledVertical -= HandleMouseScrolled;
		m_keyboard.OnKeyPressed         -= HandleKeyPressed;
		m_keyboard.OnKeyReleased        -= HandleKeyReleased;
		m_keyboard.OnKeyTyped           -= HandleKeyTyped;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~MenuController()
	{
		ReleaseUnmanagedResources();
	}

	public void Update(in VideoRect viewport)
	{
		var popupMinWidth   = m_menus.ActiveDialog?.MinimumWidth ?? 0;
		var popupMinHeight  = m_menus.ActiveDialog?.MinimumHeight ?? 0;
		var popupMaxWidth   = m_menus.ActiveDialog?.MaximumWidth ?? int.MaxValue;
		var popupMaxHeight  = m_menus.ActiveDialog?.MaximumHeight ?? int.MaxValue;
		var menuMinWidth    = m_menus.ActiveMenu?.MinimumWidth ?? 0;
		var menuMinHeight   = m_menus.ActiveMenu?.MinimumHeight ?? 0;
		var menuMaxWidth    = m_menus.ActiveMenu?.MaximumWidth ?? int.MaxValue;
		var menuMaxHeight   = m_menus.ActiveMenu?.MaximumHeight ?? int.MaxValue;
		var centerMinWidth  = m_menus.ActivePopup?.MinimumWidth ?? 0;
		var centerMinHeight = m_menus.ActivePopup?.MinimumHeight ?? 0;
		var centerMaxWidth  = m_menus.ActivePopup?.MaximumWidth ?? 0;
		var centerMaxHeight = m_menus.ActivePopup?.MaximumHeight ?? 0;

		var halfWidth   = viewport.Width / 2;
		var thirdHeight = viewport.Height / 3;
		var thirdWidth  = viewport.Width / 3;

		var statusHeight    = Math.Max(viewport.Height / 6, MINIMUM_STATUS_PANEL_HEIGHT);
		var sidePanelHeight = Math.Clamp(viewport.Height - statusHeight, MINIMUM_SIDE_PANEL_HEIGHT, MAXIMUM_SIDE_PANEL_HEIGHT);
		var sidePanelWidth  = Math.Clamp(halfWidth, MINIMUM_SIDE_PANEL_WIDTH, MAXIMUM_SIDE_PANEL_WIDTH);

		var dialogWidth  = Math.Clamp(Math.Max(thirdWidth, MINIMUM_AUTO_POPUP_WIDTH), popupMinWidth, popupMaxWidth);
		var dialogHeight = Math.Clamp(thirdHeight, popupMinHeight, popupMaxHeight);
		var menuWidth    = Math.Clamp(Math.Max(thirdWidth, MINIMUM_AUTO_POPUP_WIDTH), menuMinWidth, menuMaxWidth);
		var menuHeight   = Math.Clamp(thirdHeight, menuMinHeight, menuMaxHeight);
		var popupWidth   = Math.Clamp(Math.Max(thirdWidth, MINIMUM_AUTO_POPUP_WIDTH), centerMinWidth, centerMaxWidth);
		var popupHeight  = Math.Clamp(thirdHeight, centerMinHeight, centerMaxHeight);

		m_leftPanelRect = new VideoRect
		{
			Left   = 0,
			Top    = 0,
			Width  = sidePanelWidth,
			Height = sidePanelHeight
		};

		m_rightPanelRect = new VideoRect
		{
			Left   = Math.Max(viewport.Width - sidePanelWidth, sidePanelWidth),
			Top    = 0,
			Width  = sidePanelWidth,
			Height = sidePanelHeight
		};

		m_bottomPanelRect = new VideoRect
		{
			Left   = 0,
			Top    = Math.Max(sidePanelHeight, viewport.Height - statusHeight),
			Width  = viewport.Width,
			Height = statusHeight
		};

		m_dialogRect = new VideoRect
		{
			Left   = Math.Max(halfWidth - dialogWidth / 2, 0),
			Top    = Math.Max(thirdHeight - dialogHeight / 2, 0),
			Width  = dialogWidth,
			Height = dialogHeight
		};

		m_menuRect = new VideoRect
		{
			Left   = Math.Max(halfWidth - menuWidth / 2, 0),
			Top    = Math.Max(thirdHeight - menuHeight, 0),
			Width  = menuWidth,
			Height = menuHeight
		};

		m_popupPanelRect = new VideoRect
		{
			Left   = Math.Max(halfWidth - dialogWidth / 2, 0),
			Top    = Math.Max(thirdHeight - dialogHeight / 2, 0),
			Width  = popupWidth,
			Height = popupHeight
		};

		m_menus.ActiveMenu?.UpdateLayout(m_menuRect);
		m_menus.ActiveDialog?.UpdateLayout(m_dialogRect);
		m_menus.ActivePopup?.UpdateLayout(m_popupPanelRect);
		m_menus.LeftSidePanel?.UpdateLayout(m_leftPanelRect);
		m_menus.RightSidePanel?.UpdateLayout(m_rightPanelRect);
		m_menus.BottomPanel?.UpdateLayout(m_bottomPanelRect);
	}

	public void Display(in VideoRect viewport)
	{
		var redColor    = new VideoColor { Red  = 200, Green = 55, Blue  = 55, Alpha  = 255 };
		var greenColor  = new VideoColor { Red  = 55, Green  = 178, Blue = 55, Alpha  = 255 };
		var blueColor   = new VideoColor { Red  = 55, Green  = 98, Blue  = 150, Alpha = 255 };
		var yellowColor = new VideoColor { Red  = 176, Green = 176, Blue = 55, Alpha  = 255 };
		var srcRect     = new VideoRect { Width = 8, Height  = 8 };

		m_textures.BindOrIgnore(m_assets.GetSolidColorImage(), texture =>
		{
			texture.Blit(m_leftPanelRect, srcRect, redColor, VideoBlitFlags.None);
			texture.Blit(m_rightPanelRect, srcRect, greenColor, VideoBlitFlags.None);
			texture.Blit(m_bottomPanelRect, srcRect, blueColor, VideoBlitFlags.None);
			texture.Blit(m_dialogRect, srcRect, yellowColor, VideoBlitFlags.None);
		});
	}

	private void HandleMouseMoved(IPlatformMouse mouse, int left, int top)
	{
		if (m_menus.ActiveMenu != null)
		{
			m_focusedPanel = m_menus.ActiveMenu;
		}
		else if (m_menus.ActiveDialog != null)
		{
			m_focusedPanel = m_menus.ActiveDialog;
		}
		else if (m_menus.ActivePopup != null)
		{
			m_focusedPanel = m_menus.ActivePopup;
		}
		else
		{
			var mouseRect = new VideoRect { Left = left, Top = top, Width = 4, Height = 4 };
			if (m_menus.LeftSidePanel != null && m_leftPanelRect.Intersects(mouseRect))
			{
				m_focusedPanel = m_menus.LeftSidePanel;
			}
			else if (m_menus.RightSidePanel != null && m_rightPanelRect.Intersects(mouseRect))
			{
				m_focusedPanel = m_menus.RightSidePanel;
			}
			else if (m_menus.BottomPanel != null && m_bottomPanelRect.Intersects(mouseRect))
			{
				m_focusedPanel = m_menus.BottomPanel;
			}
			else
			{
				m_focusedPanel = null;
			}
		}
	}

	private void HandleMousePressed(IPlatformMouse mouse, int left, int top, MouseButton button)
	{
	}

	private void HandleMouseReleased(IPlatformMouse mouse, int left, int top, MouseButton button)
	{
	}

	private void HandleMouseScrolled(IPlatformMouse mouse, int left, int top, int offset)
	{
	}

	private void HandleKeyPressed(IPlatformKeyboard keyboard, KeyboardKey key)
	{
	}

	private void HandleKeyReleased(IPlatformKeyboard keyboard, KeyboardKey key)
	{
	}

	private void HandleKeyTyped(IPlatformKeyboard keyboard, char character)
	{
	}
}
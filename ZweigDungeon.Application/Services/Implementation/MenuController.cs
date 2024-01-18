using ZweigDungeon.Application.Entities;
using ZweigDungeon.Application.Entities.Menu;
using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigDungeon.Application.Entities.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuController : IDisposable, IMenuController
{
	private readonly CurrentScene      m_scene;
	private readonly IPlatformKeyboard m_keyboard;
	private readonly IPlatformMouse    m_mouse;

	public MenuController(CurrentScene scene, IPlatformKeyboard keyboard, IPlatformMouse mouse)
	{
		m_scene                  =  scene;
		m_keyboard               =  keyboard;
		m_mouse                  =  mouse;
		m_keyboard.OnKeyPressed  += HandleKeyPressed;
		m_keyboard.OnKeyReleased += HandleKeyReleased;
		m_keyboard.OnKeyTyped    += HandleKeyTyped;
		m_mouse.OnMouseMoved     += HandleMouseMoved;
		m_mouse.OnMousePressed   += HandleMousePressed;
		m_mouse.OnMouseReleased  += HandleMouseReleased;
	}

	private void ReleaseUnmanagedResources()
	{
		m_keyboard.OnKeyPressed  -= HandleKeyPressed;
		m_keyboard.OnKeyReleased -= HandleKeyReleased;
		m_keyboard.OnKeyTyped    -= HandleKeyTyped;
		m_mouse.OnMouseMoved     -= HandleMouseMoved;
		m_mouse.OnMousePressed   -= HandleMousePressed;
		m_mouse.OnMouseReleased  -= HandleMouseReleased;
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

	public void DisplayStartupMenu()
	{
		var startupMenu = new ControlPanel();

		startupMenu.Children.Add(new ButtonControl
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment   = VerticalAlignment.Top,
			MarginLeft          = 10,
			MarginTop           = 10,
			MarginRight         = 10,
			MarginBottom        = 10,
			MinimumWidth        = 500,
			MaximumWidth        = 500,
			MinimumHeight       = 30,
			MaximumHeight       = 30,
			LabelColor          = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 },
			Label               = "Look! It's moving. It's alive. It's alive..."
		});

		m_scene.MenuPanel = startupMenu;
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

	private void HandleMouseMoved(IPlatformMouse mouse, int left, int top)
	{
	}

	private void HandleMousePressed(IPlatformMouse mouse, int left, int top, MouseButton button)
	{
	}

	private void HandleMouseReleased(IPlatformMouse mouse, int left, int top, MouseButton button)
	{
	}
}
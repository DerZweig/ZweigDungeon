using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Messages;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application;

public class App : IDisposable, IWindowListener
{
	private readonly IMenuRepository m_menuRepository;
	private readonly IFontRepository m_fontRepository;
	private readonly IDisposable     m_subscription;
	private          MenuDefinition? m_menuActive;
	private          FontDefinition? m_smallFont;
	private          FontDefinition? m_mediumFont;
	private          FontDefinition? m_largeFont;

	public App(MessageBus messageBus, IMenuRepository menuRepository, IFontRepository fontRepository)
	{
		m_menuRepository = menuRepository;
		m_fontRepository = fontRepository;
		m_subscription   = messageBus.Subscribe<IWindowListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_subscription.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~App()
	{
		ReleaseUnmanagedResources();
	}

	public async void WindowCreated(IPlatformWindow window)
	{
		window.SetTitle("ZweigDungeon");
		window.SetStyle(true, true);
		window.SetMinimumSize(640, 480);
		window.Show();

		m_smallFont  = await m_fontRepository.LoadFont("Gui/font_small");
		m_mediumFont = await m_fontRepository.LoadFont("Gui/font_medium");
		m_largeFont  = await m_fontRepository.LoadFont("Gui/font_large");
		m_menuActive = await m_menuRepository.LoadMenu("Menu/StartupMenu");
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
		var width  = window.GetViewportWidth();
		var height = window.GetViewportHeight();

		if (m_menuActive != null && m_smallFont != null && m_mediumFont != null && m_largeFont != null)
		{
			m_menuActive.UpdateLayout(new VideoRect { Left = 0, Top = 0, Width = width, Height = height },
			                          new MenuStyle
			                          {
				                          SmallFont  = m_smallFont,
				                          MediumFont = m_mediumFont,
				                          LargeFont  = m_largeFont
			                          });
		}
	}
}
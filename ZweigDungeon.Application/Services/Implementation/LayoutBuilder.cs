using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Entities.Assets.Menu.Constants;
using ZweigDungeon.Application.Entities.Assets.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Platform.Messages;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Messages;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Services.Implementation;

public class LayoutBuilder : IDisposable, IWindowListener, ILayoutBuilder
{
	private readonly IFontRepository    m_fontRepository;
	private readonly IDisposable     m_subscription;
	private          FontDefinition? m_small;
	private          FontDefinition? m_medium;
	private          FontDefinition? m_large;

	public LayoutBuilder(IFontRepository fontRepository, MessageBus messageBus)
	{
		m_fontRepository  = fontRepository;
		m_subscription = messageBus.Subscribe<IWindowListener>(this);
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

	~LayoutBuilder()
	{
		ReleaseUnmanagedResources();
	}

	public async void WindowCreated(IPlatformWindow window)
	{
		m_small  = await m_fontRepository.LoadFont("Gui/font_large.fnt");
		m_medium = await m_fontRepository.LoadFont("Gui/font_medium.fnt");
		m_large  = await m_fontRepository.LoadFont("Gui/font_large.fnt");
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
	}

	public void UpdateLayout(MenuDefinition menu, in VideoRect viewport)
	{
		if (m_small == null || m_medium == null || m_large == null)
		{
			return;
		}

		UpdateControlLayout(menu.Panel, viewport);

		var pending = new Queue<BasicControl>();
		pending.Enqueue(menu.Panel);
		while (pending.TryDequeue(out var control))
		{
			switch (control)
			{
				case PanelControl panel:
				{
					LayoutChildren(panel.Layout, panel.LayoutRect, panel.Children);
					foreach (var child in panel.Children)
					{
						pending.Enqueue(child);
					}

					break;
				}
				case ButtonControl button:
				{
					LayoutChildren(PanelLayout.None, button.LayoutRect, button.Children);
					foreach (var child in button.Children)
					{
						pending.Enqueue(child);
					}

					break;
				}
			}
		}
	}

	private void LayoutChildren(PanelLayout layout, in VideoRect viewport, IReadOnlyList<BasicControl> children)
	{
		if (!children.Any())
		{
			return;
		}

		if (layout == PanelLayout.None)
		{
			foreach (var child in children)
			{
				UpdateControlLayout(child, viewport);
			}
		}
		else if (layout == PanelLayout.Horizontal)
		{
			var desiredWidths  = new List<int>();
			var weightedWidths = new List<int>();

			foreach (var child in children)
			{
				var marginLeft   = child.MarginLeft ?? 0;
				var marginRight  = child.MarginRight ?? 0;
				var minimumWidth = child.MinimumWidth ?? 0;
				var maximumWidth = child.MaximumWidth ?? int.MaxValue;
				desiredWidths.Add(Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth));
			}

			for (var index = 0; index < children.Count; ++index)
			{
				var child         = children[index];
				var weighting     = (float)desiredWidths[index] / viewport.Width / children.Count;
				var marginLeft    = child.MarginLeft ?? 0;
				var marginRight   = child.MarginRight ?? 0;
				var minimumWidth  = child.MinimumWidth ?? 0;
				var maximumWidth  = child.MaximumWidth ?? int.MaxValue;
				var expectedWidth = viewport.Width - marginLeft - marginRight;

				expectedWidth = (int)(expectedWidth * weighting);
				expectedWidth = Math.Clamp(expectedWidth, minimumWidth, maximumWidth);

				weightedWidths.Add(expectedWidth);
			}

			var viewportWork = viewport;
			for (var index = 0; index < children.Count; ++index)
			{
				var child = children[index];
				var width = weightedWidths[index];

				viewportWork.Width = width;
				UpdateControlLayout(child, viewportWork);
				viewportWork.Left = child.LayoutRect.Left + child.LayoutRect.Width;
			}
		}
		else if (layout == PanelLayout.Vertical)
		{
			var desiredHeights  = new List<int>();
			var weightedHeights = new List<int>();

			foreach (var child in children)
			{
				var marginTop     = child.MarginTop ?? 0;
				var marginBottom  = child.MarginBottom ?? 0;
				var minimumHeight = child.MinimumHeight ?? 0;
				var maximumHeight = child.MaximumHeight ?? int.MaxValue;
				desiredHeights.Add(Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight));
			}

			for (var index = 0; index < children.Count; ++index)
			{
				var child          = children[index];
				var weighting      = (float)desiredHeights[index] / viewport.Height / children.Count;
				var marginTop      = child.MarginTop ?? 0;
				var marginBottom   = child.MarginBottom ?? 0;
				var minimumHeight  = child.MinimumHeight ?? 0;
				var maximumHeight  = child.MaximumHeight ?? int.MaxValue;
				var expectedHeight = viewport.Height - marginTop - marginBottom;

				expectedHeight = (int)(expectedHeight * weighting);
				expectedHeight = Math.Clamp(expectedHeight, minimumHeight, maximumHeight);
				weightedHeights.Add(expectedHeight);
			}

			var viewportWork = viewport;
			for (var index = 0; index < children.Count; ++index)
			{
				var child  = children[index];
				var height = weightedHeights[index];

				viewportWork.Height = height;
				UpdateControlLayout(child, viewportWork);
				viewportWork.Top = child.LayoutRect.Top + child.LayoutRect.Height;
			}
		}
	}

	private void UpdateControlLayout(BasicControl control, in VideoRect viewport)
	{
		var marginLeft    = control.MarginLeft ?? 0;
		var marginRight   = control.MarginRight ?? 0;
		var marginTop     = control.MarginTop ?? 0;
		var marginBottom  = control.MarginBottom ?? 0;
		var minimumWidth  = control.MinimumWidth ?? 0;
		var maximumWidth  = control.MaximumWidth ?? int.MaxValue;
		var minimumHeight = control.MinimumHeight ?? 0;
		var maximumHeight = control.MaximumHeight ?? int.MaxValue;

		var width  = Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth);
		var height = Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight);

		var left = control.HorizontalAlignment switch
		           {
			           HorizontalAlignment.Left => 0,
			           HorizontalAlignment.Center => viewport.Width / 2 - width / 2,
			           HorizontalAlignment.Right => viewport.Width - width - marginRight,
			           _ => throw new UnhandledEnumException<HorizontalAlignment>(control.HorizontalAlignment)
		           };
		var top = control.VerticalAlignment switch
		          {
			          VerticalAlignment.Top => 0,
			          VerticalAlignment.Center => viewport.Height / 2 - height / 2,
			          VerticalAlignment.Bottom => viewport.Height - height - marginBottom,
			          _ => throw new UnhandledEnumException<VerticalAlignment>(control.VerticalAlignment)
		          };

		left   = Math.Max(left, marginLeft);
		top    = Math.Max(top, marginTop);
		width  = Math.Min(left + width, viewport.Width) - left;
		height = Math.Min(top + height, viewport.Height) - top;

		control.LayoutRect = new VideoRect
		{
			Left   = left + viewport.Left,
			Top    = top + viewport.Top,
			Width  = width,
			Height = height
		};
	}
}
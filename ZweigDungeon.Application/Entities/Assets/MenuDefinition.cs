using ZweigDungeon.Application.Entities.Assets.Menu.Constants;
using ZweigDungeon.Application.Entities.Assets.Menu.Controls;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Entities.Assets;

public class MenuDefinition
{
	private VideoRect       m_viewport;
	private FontDefinition? m_smallFont;
	private FontDefinition? m_mediumFont;
	private FontDefinition? m_largeFont;

	public MenuDefinition()
	{
		Panel = new PanelControl();
	}

	public PanelControl Panel { get; }

	public void UpdateLayout(in VideoRect viewport, MenuStyle style)
	{
		if (viewport.Left == m_viewport.Left &&
		    viewport.Top == m_viewport.Top &&
		    viewport.Width == m_viewport.Width &&
		    viewport.Height == m_viewport.Height &&
		    style.SmallFont == m_smallFont &&
		    style.MediumFont == m_mediumFont &&
		    style.LargeFont == m_largeFont)
		{
			return;
		}

		m_viewport   = viewport;
		m_smallFont  = style.SmallFont;
		m_mediumFont = style.MediumFont;
		m_largeFont  = style.LargeFont;

		UpdateControlLayout(Panel, viewport);

		var pending = new Queue<BasicControl>();
		var texts   = new Queue<TextControl>();
		pending.Enqueue(Panel);
		
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
				case TextControl text:
				{
					texts.Enqueue(text);
					break;
				}
			}
		}

		while (texts.TryDequeue(out var text))
		{
			var width = text.LayoutRect.Width;
			text.LayoutText = text.FontSize switch
			                  {
				                  FontSize.Small => m_smallFont.LayoutString(text.Text, width),
				                  FontSize.Medium => m_mediumFont.LayoutString(text.Text, width),
				                  FontSize.Large => m_largeFont.LayoutString(text.Text, width),
				                  _ => throw new UnhandledEnumException<FontSize>(text.FontSize)
			                  };
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
using System.Text;
using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Entities.Assets.Font;
using ZweigDungeon.Application.Entities.Assets.Menu.Constants;
using ZweigDungeon.Application.Entities.Assets.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Interfaces.Platform.Messages;
using ZweigEngine.Common.Services.Messages;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuRenderer : IDisposable, IWindowListener, IMenuRenderer
{
	private readonly IDisposable                           m_subscription;
	private readonly IFontRepository                       m_fontRepository;
	private readonly IImageRepository                      m_imageRepository;
	private readonly ITextureManager                       m_textureManager;
	private readonly Dictionary<MenuDefinition, MenuEntry> m_processed;
	private          FontDefinition?                       m_smallFontDefinition;
	private          FontDefinition?                       m_mediumFontDefinition;
	private          FontDefinition?                       m_largeFontDefinition;
	private          Image?                                m_smallFontImage;
	private          Image?                                m_mediumFontImage;
	private          Image?                                m_largeFontImage;

	public MenuRenderer(MessageBus messageBus, IFontRepository fontRepository, IImageRepository imageRepository, ITextureManager textureManager)
	{
		m_subscription    = messageBus.Subscribe<IWindowListener>(this);
		m_fontRepository  = fontRepository;
		m_imageRepository = imageRepository;
		m_textureManager  = textureManager;
		m_processed       = new Dictionary<MenuDefinition, MenuEntry>();
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

	~MenuRenderer()
	{
		ReleaseUnmanagedResources();
	}

	public async void WindowCreated(IPlatformWindow window)
	{
		m_smallFontDefinition  = await m_fontRepository.LoadFont("Gui/font_small");
		m_mediumFontDefinition = await m_fontRepository.LoadFont("Gui/font_medium");
		m_largeFontDefinition  = await m_fontRepository.LoadFont("Gui/font_large");
		m_smallFontImage       = await m_imageRepository.LoadImage("Gui/font_small");
		m_mediumFontImage      = await m_imageRepository.LoadImage("Gui/font_medium");
		m_largeFontImage       = await m_imageRepository.LoadImage("Gui/font_large");

		m_textureManager.Upload(m_smallFontImage);
		m_textureManager.Upload(m_mediumFontImage);
		m_textureManager.Upload(m_largeFontImage);
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
	}

	public void Draw(MenuDefinition menu, in VideoRect viewport)
	{
		if (m_smallFontDefinition == null ||
		    m_mediumFontDefinition == null ||
		    m_largeFontDefinition == null ||
		    m_smallFontImage == null ||
		    m_mediumFontImage == null ||
		    m_largeFontImage == null)
		{
			return;
		}

		if (!m_processed.TryGetValue(menu, out var cached))
		{
			cached = new MenuEntry();
			m_processed.Add(menu, cached);

			BuildHierarchy(cached, menu);
		}

		DrawLayout(cached, viewport);
	}

	private void DrawLayout(MenuEntry cached, in VideoRect viewport)
	{
		UpdateLayout(cached, viewport);
		var foreground = new VideoColor { Red = 255, Green = 255, Blue = 255, Alpha = 255 };
		foreach (var entry in cached.Controls)
		{
			if (entry is TextEntry text)
			{
				switch (text.FontSize)
				{
					case FontSize.Small:
						m_textureManager.Bind(m_smallFontImage!, texture => DrawText(m_smallFontDefinition!, texture, text, foreground));
						break;
					case FontSize.Medium:
						m_textureManager.Bind(m_mediumFontImage!, texture => DrawText(m_mediumFontDefinition!, texture, text, foreground));
						break;
					case FontSize.Large:
						m_textureManager.Bind(m_largeFontImage!, texture => DrawText(m_largeFontDefinition!, texture, text, foreground));
						break;
					default:
						throw new UnhandledEnumException<FontSize>(text.FontSize);
				}
			}
		}
	}

	private void UpdateLayout(MenuEntry cached, in VideoRect viewport)
	{
		if (cached.Viewport.Left == viewport.Left &&
		    cached.Viewport.Top == viewport.Top &&
		    cached.Viewport.Width == viewport.Width &&
		    cached.Viewport.Height == viewport.Height)
		{
			return;
		}

		if (!cached.Panels.Any())
		{
			return;
		}

		cached.Viewport = viewport;
		UpdateControlLayoutRect(cached.Panels.First(), viewport);

		foreach (var panel in cached.Panels)
		{
			UpdatePanelLayoutChildren(panel.Layout, panel.Rect, panel.Children);
		}

		var texts = cached.Texts;

		UpdateTextLayoutString(texts);
	}

	private static void BuildHierarchy(MenuEntry cached, MenuDefinition menu)
	{
		var unwind = new Queue<PanelEntry>();
		unwind.Enqueue(new PanelEntry(menu.Panel)
		{
			Layout = menu.Panel.Layout
		});

		while (unwind.TryDequeue(out var node))
		{
			cached.Controls.Add(node);
			cached.Panels.Add(node);

			var childControls = node.Definition switch
			                    {
				                    PanelControl panel => panel.Children,
				                    ButtonControl button => button.Children,
				                    _ => throw new NotImplementedException($"Unexpected panel type {node.Definition.GetType().Name}")
			                    };

			foreach (var childControl in childControls)
			{
				switch (childControl)
				{
					case PanelControl panel:
					{
						var entry = new PanelEntry(panel) { Layout = panel.Layout };
						cached.Panels.Add(entry);
						cached.Controls.Add(entry);
						node.Children.Add(entry);
						unwind.Enqueue(entry);
						break;
					}
					case ButtonControl button:
					{
						var entry = new PanelEntry(button) { Layout = PanelLayout.None };
						cached.Panels.Add(entry);
						cached.Controls.Add(entry);
						node.Children.Add(entry);
						unwind.Enqueue(entry);
						break;
					}
					case TextControl text:
					{
						var entry = new TextEntry(text) { FontSize = text.FontSize };
						cached.Controls.Add(entry);
						cached.Texts.Add(entry);
						node.Children.Add(entry);
						break;
					}
					default:
					{
						var entry = new ControlEntry(childControl);
						cached.Controls.Add(entry);
						node.Children.Add(entry);
						break;
					}
				}
			}
		}

		foreach (var entry in cached.Controls)
		{
			entry.HorizontalAlignment = entry.Definition.HorizontalAlignment;
			entry.VerticalAlignment   = entry.Definition.VerticalAlignment;
			entry.MinimumWidth        = entry.Definition.MinimumWidth ?? 0;
			entry.MinimumHeight       = entry.Definition.MinimumHeight ?? 0;
			entry.MaximumWidth        = entry.Definition.MaximumWidth ?? int.MaxValue;
			entry.MaximumHeight       = entry.Definition.MaximumHeight ?? int.MaxValue;
			entry.MarginLeft          = entry.Definition.MarginLeft ?? 0;
			entry.MarginTop           = entry.Definition.MarginTop ?? 0;
			entry.MarginRight         = entry.Definition.MarginRight ?? 0;
			entry.MarginBottom        = entry.Definition.MarginBottom ?? 0;
		}
	}

	private static void UpdatePanelLayoutChildren(PanelLayout layout, in VideoRect viewport, IReadOnlyList<ControlEntry> children)
	{
		if (!children.Any())
		{
			return;
		}

		switch (layout)
		{
			case PanelLayout.None:
			{
				foreach (var child in children)
				{
					UpdateControlLayoutRect(child, viewport);
				}

				break;
			}
			case PanelLayout.Horizontal:
			{
				var desiredWidths  = new List<int>();
				var weightedWidths = new List<int>();

				foreach (var child in children)
				{
					var marginLeft   = child.MarginLeft;
					var marginRight  = child.MarginRight;
					var minimumWidth = child.MinimumWidth;
					var maximumWidth = child.MaximumWidth;
					desiredWidths.Add(Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth));
				}

				for (var index = 0; index < children.Count; ++index)
				{
					var child         = children[index];
					var weighting     = (float)desiredWidths[index] / viewport.Width / children.Count;
					var marginLeft    = child.MarginLeft;
					var marginRight   = child.MarginRight;
					var minimumWidth  = child.MinimumWidth;
					var maximumWidth  = child.MaximumWidth;
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
					UpdateControlLayoutRect(child, viewportWork);
					viewportWork.Left = child.Rect.Left + child.Rect.Width;
				}

				break;
			}
			case PanelLayout.Vertical:
			{
				var desiredHeights  = new List<int>();
				var weightedHeights = new List<int>();

				foreach (var child in children)
				{
					var marginTop     = child.MarginTop;
					var marginBottom  = child.MarginBottom;
					var minimumHeight = child.MinimumHeight;
					var maximumHeight = child.MaximumHeight;
					desiredHeights.Add(Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight));
				}

				for (var index = 0; index < children.Count; ++index)
				{
					var child          = children[index];
					var weighting      = (float)desiredHeights[index] / viewport.Height / children.Count;
					var marginTop      = child.MarginTop;
					var marginBottom   = child.MarginBottom;
					var minimumHeight  = child.MinimumHeight;
					var maximumHeight  = child.MaximumHeight;
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
					UpdateControlLayoutRect(child, viewportWork);
					viewportWork.Top = child.Rect.Top + child.Rect.Height;
				}

				break;
			}
		}
	}

	private void UpdateTextLayoutString(List<TextEntry> texts)
	{
		var resultLines = new List<string>();
		var lineBuilder = new StringBuilder();
		foreach (var entry in texts)
		{
			var width = entry.Rect.Width;
			var font = entry.FontSize switch
			           {
				           FontSize.Small => m_smallFontDefinition!,
				           FontSize.Medium => m_mediumFontDefinition!,
				           FontSize.Large => m_largeFontDefinition!,
				           _ => throw new UnhandledEnumException<FontSize>(entry.FontSize)
			           };

			entry.Lines.Clear();

			resultLines.Clear();
			lineBuilder.Clear();
			using (var reader = new StringReader(((TextControl)entry.Definition).Text))
			{
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
				{
					var words     = new List<string>();
					var wordStart = 0;
					var wordSpace = false;

					line = line.Trim();
					for (var index = 0; index < line.Length; ++index)
					{
						var curSpace = char.IsWhiteSpace(line[index]);
						if (curSpace != wordSpace)
						{
							words.Add(line.Substring(wordStart, index - wordStart));
							wordStart = index;
							wordSpace = curSpace;
						}
					}

					if (wordStart < line.Length)
					{
						words.Add(line.Substring(wordStart));
					}

					var cursorWidth = 0;

					foreach (var word in words)
					{
						var wordSize = MeasureString(font, word);
						if (wordSize + cursorWidth >= width)
						{
							cursorWidth = 0;
							resultLines.Add(lineBuilder.ToString());
							lineBuilder.Clear();
							if (string.IsNullOrWhiteSpace(word))
							{
								continue;
							}
						}

						cursorWidth += wordSize;
						lineBuilder.Append(word);
					}

					resultLines.Add(lineBuilder.ToString());
					lineBuilder.Clear();
				}
			}

			while (resultLines.Any() && string.IsNullOrWhiteSpace(resultLines.Last()))
			{
				resultLines.RemoveAt(resultLines.Count - 1);
			}

			foreach (var line in resultLines)
			{
				var trimmed = line.Trim();
				entry.Lines.Add(new TextLine(trimmed, MeasureString(font, trimmed)));
			}
		}
	}

	private static void UpdateControlLayoutRect(ControlEntry controlEntry, in VideoRect viewport)
	{
		var marginLeft    = controlEntry.MarginLeft;
		var marginRight   = controlEntry.MarginRight;
		var marginTop     = controlEntry.MarginTop;
		var marginBottom  = controlEntry.MarginBottom;
		var minimumWidth  = controlEntry.MinimumWidth;
		var minimumHeight = controlEntry.MinimumHeight;
		var maximumWidth  = controlEntry.MaximumWidth;
		var maximumHeight = controlEntry.MaximumHeight;

		var width  = Math.Clamp(viewport.Width - marginLeft - marginRight, minimumWidth, maximumWidth);
		var height = Math.Clamp(viewport.Height - marginTop - marginBottom, minimumHeight, maximumHeight);

		var left = controlEntry.HorizontalAlignment switch
		           {
			           HorizontalAlignment.Left => 0,
			           HorizontalAlignment.Center => viewport.Width / 2 - width / 2,
			           HorizontalAlignment.Right => viewport.Width - width - marginRight,
			           _ => throw new UnhandledEnumException<HorizontalAlignment>(controlEntry.HorizontalAlignment)
		           };
		var top = controlEntry.VerticalAlignment switch
		          {
			          VerticalAlignment.Top => 0,
			          VerticalAlignment.Center => viewport.Height / 2 - height / 2,
			          VerticalAlignment.Bottom => viewport.Height - height - marginBottom,
			          _ => throw new UnhandledEnumException<VerticalAlignment>(controlEntry.VerticalAlignment)
		          };

		left   = Math.Max(left, marginLeft);
		top    = Math.Max(top, marginTop);
		width  = Math.Min(left + width, viewport.Width) - left;
		height = Math.Min(top + height, viewport.Height) - top;

		controlEntry.Rect = new VideoRect
		{
			Left   = left + viewport.Left,
			Top    = top + viewport.Top,
			Width  = width,
			Height = height
		};
	}

	private static int MeasureString(FontDefinition fontDefinition, string word)
	{
		var wordSize = 0;
		using (var enumerator = word.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				var currentChar = enumerator.Current;
				while (enumerator.MoveNext())
				{
					var nextChar = enumerator.Current;
					var kerning  = new FontKerning(currentChar, nextChar);
					wordSize += MeasureCharacter(fontDefinition, currentChar);

					if (fontDefinition.Kernings.TryGetValue(kerning, out var amount))
					{
						wordSize += amount;
					}

					currentChar = nextChar;
				}

				wordSize += MeasureCharacter(fontDefinition, currentChar);
			}
		}

		return wordSize;
	}

	private static int MeasureCharacter(FontDefinition fontDefinition, char character)
	{
		if (fontDefinition.Chars.TryGetValue(character, out var charInfo) || fontDefinition.Chars.TryGetValue(' ', out charInfo))
		{
			return charInfo.Advance + charInfo.OffsetLeft;
		}

		return 0;
	}

	private static void DrawText(FontDefinition font, IVideoImage texture, TextEntry entry, in VideoColor color)
	{
		var top  = entry.Rect.Top;
		var clip = entry.Rect;
		switch (entry.HorizontalAlignment)
		{
			case HorizontalAlignment.Center:
				foreach (var line in entry.Lines)
				{
					var text = line.Text;
					var left = clip.Left + clip.Width / 2 - line.Width / 2;

					DrawString(font, texture, text, left, top, color, clip);

					top += font.LineHeight;
				}

				break;
			case HorizontalAlignment.Right:
				foreach (var line in entry.Lines)
				{
					var text = line.Text;
					var left = clip.Left + clip.Width - line.Width;

					DrawString(font, texture, text, left, top, color, clip);

					top += font.LineHeight;
				}

				break;
			case HorizontalAlignment.Left:
				foreach (var line in entry.Lines)
				{
					var text = line.Text;
					var left = clip.Left;

					DrawString(font, texture, text, left, top, color, clip);

					top += font.LineHeight;
				}

				break;
		}
	}

	private static void DrawString(FontDefinition font, IVideoImage texture, string text, int left, int top, in VideoColor color, in VideoRect clip)
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
					left += DrawCharacter(font, currentChar, texture, left, top, clip, color);

					if (font.Kernings.TryGetValue(kerning, out var amount))
					{
						left += amount;
					}

					currentChar = nextChar;
				}

				DrawCharacter(font, currentChar, texture, left, top, clip, color);
			}
		}
	}

	private static int DrawCharacter(FontDefinition font, char character, IVideoImage texture, int left, int top, in VideoRect clip, in VideoColor color)
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

	private class MenuEntry
	{
		public MenuEntry()
		{
			Controls = new List<ControlEntry>();
			Panels   = new List<PanelEntry>();
			Texts    = new List<TextEntry>();
		}

		public List<ControlEntry> Controls { get; }
		public List<PanelEntry>   Panels   { get; }
		public List<TextEntry>    Texts    { get; }
		public VideoRect          Viewport { get; set; }
	}

	private class PanelEntry : ControlEntry
	{
		public PanelEntry(BasicControl definition) : base(definition)
		{
			Children = new List<ControlEntry>();
		}

		public PanelLayout        Layout   { get; set; }
		public List<ControlEntry> Children { get; }
	}

	private class TextEntry : ControlEntry
	{
		public TextEntry(BasicControl definition) : base(definition)
		{
			Lines = new List<TextLine>();
		}

		public List<TextLine> Lines    { get; }
		public FontSize       FontSize { get; set; }
	}

	private class ControlEntry
	{
		public ControlEntry(BasicControl definition)
		{
			Definition = definition;
		}

		public BasicControl        Definition          { get; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment   VerticalAlignment   { get; set; }
		public int                 MinimumWidth        { get; set; }
		public int                 MinimumHeight       { get; set; }
		public int                 MaximumWidth        { get; set; }
		public int                 MaximumHeight       { get; set; }
		public int                 MarginLeft          { get; set; }
		public int                 MarginTop           { get; set; }
		public int                 MarginRight         { get; set; }
		public int                 MarginBottom        { get; set; }
		public VideoRect           Rect                { get; set; }
	}

	private class TextLine
	{
		public TextLine(string text, int width)
		{
			Text  = text;
			Width = width;
		}

		public string Text  { get; }
		public int    Width { get; }
	}
}
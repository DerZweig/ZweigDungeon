using System.Text;
using ZweigDungeon.Application.Manager.Constants;
using ZweigDungeon.Application.Manager.Interfaces;
using ZweigEngine.Common.Interfaces.Platform;
using ZweigEngine.Common.Interfaces.Platform.Messages;
using ZweigEngine.Common.Interfaces.Video;
using ZweigEngine.Common.Services.Messages;

namespace ZweigDungeon.Application.Manager.Implementation;

public class FontManager : IDisposable, IWindowListener, IFontManager
{
	private const string FONT_IMAGE_SMALL       = "Gui/font_small";
	private const string FONT_IMAGE_MEDIUM      = "Gui/font_medium";
	private const string FONT_IMAGE_LARGE       = "Gui/font_large";
	private const string FONT_DEFINITION_SMALL  = "Data/Gui/font_small.fnt";
	private const string FONT_DEFINITION_MEDIUM = "Data/Gui/font_medium.fnt";
	private const string FONT_DEFINITION_LARGE  = "Data/Gui/font_large.fnt";

	private readonly IImageManager m_imageManager;
	private readonly IDisposable   m_subscription;
	private readonly FontType      m_small;
	private readonly FontType      m_medium;
	private readonly FontType      m_large;

	public FontManager(MessageBus messageBus, IImageManager imageManager)
	{
		m_imageManager = imageManager;
		m_subscription = messageBus.Subscribe<IWindowListener>(this);
		m_small        = new FontType();
		m_medium       = new FontType();
		m_large        = new FontType();
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

	~FontManager()
	{
		ReleaseUnmanagedResources();
	}

	public void WindowCreated(IPlatformWindow window)
	{
		m_imageManager.Load(FONT_IMAGE_SMALL);
		m_imageManager.Load(FONT_IMAGE_MEDIUM);
		m_imageManager.Load(FONT_IMAGE_LARGE);
		LoadFontDefinition(m_small, FONT_DEFINITION_SMALL);
		LoadFontDefinition(m_medium, FONT_DEFINITION_MEDIUM);
		LoadFontDefinition(m_large, FONT_DEFINITION_LARGE);
	}

	public void WindowClosing(IPlatformWindow window)
	{
	}

	public void WindowUpdateFrame(IPlatformWindow window)
	{
	}

	public void Layout(FontSize size, int viewportWidth, string text, out string result)
	{
		switch (size)
		{
			case FontSize.Small:
				Layout(m_small, viewportWidth, text, out result);
				break;
			case FontSize.Medium:
				Layout(m_medium, viewportWidth, text, out result);
				break;
			case FontSize.Large:
				Layout(m_large, viewportWidth, text, out result);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(size), size, null);
		}
	}

	public void Draw(FontSize size, string text, int left, int top, VideoRect clip, VideoColor color)
	{
		switch (size)
		{
			case FontSize.Small:
				m_imageManager.Bind(FONT_IMAGE_SMALL, texture => Draw(texture, m_small, text, left, top, clip, color));
				break;
			case FontSize.Medium:
				m_imageManager.Bind(FONT_IMAGE_MEDIUM, texture => Draw(texture, m_medium, text, left, top, clip, color));
				break;
			case FontSize.Large:
				m_imageManager.Bind(FONT_IMAGE_LARGE, texture => Draw(texture, m_large, text, left, top, clip, color));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(size), size, null);
		}
	}

	private void Layout(FontType type, int viewportWidth, string text, out string result)
	{
		var lineBuilder   = new StringBuilder();
		var resultBuilder = new StringBuilder();

		using (var reader = new StringReader(text))
		{
			for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
			{
				var words     = new List<string>();
				var wordStart = 0;
				var wordSpace = false;
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

				var cursorSize = 0;
				foreach (var word in words)
				{
					var wordSize = MeasureWordSize(type, word);
					if (wordSize + cursorSize >= viewportWidth)
					{
						cursorSize = 0;
						lineBuilder.AppendLine();
						if (string.IsNullOrWhiteSpace(word))
						{
							continue;
						}
					}

					cursorSize += wordSize;
					lineBuilder.Append(word);
				}

				resultBuilder.AppendLine(lineBuilder.ToString().TrimEnd());
				lineBuilder.Clear();
			}

			result = resultBuilder.ToString();
		}
	}

	private int MeasureWordSize(FontType type, string word)
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
					wordSize += MeasureCharacter(type, currentChar);

					if (type.Kernings.TryGetValue(kerning, out var amount))
					{
						wordSize += amount;
					}

					currentChar = nextChar;
				}

				wordSize += MeasureCharacter(type, currentChar);
			}
		}

		return wordSize;
	}

	private int MeasureCharacter(FontType type, char character)
	{
		if (type.Chars.TryGetValue(character, out var charInfo) || type.Chars.TryGetValue(' ', out charInfo))
		{
			return charInfo.Advance + charInfo.OffsetLeft;
		}

		return 0;
	}

	private void Draw(IVideoImage texture, FontType type, string text, int left, int top, VideoRect clip, VideoColor color)
	{
		using (var reader = new StringReader(text))
		{
			var cursorTop = top;

			for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
			{
				using (var enumerator = line.GetEnumerator())
				{
					var cursorLeft = left;

					if (enumerator.MoveNext())
					{
						var currentChar = enumerator.Current;
						while (enumerator.MoveNext())
						{
							var nextChar = enumerator.Current;
							var kerning  = new FontKerning(currentChar, nextChar);
							cursorLeft += DrawCharacter(type, currentChar, texture, cursorLeft, cursorTop, clip, color);

							if (type.Kernings.TryGetValue(kerning, out var amount))
							{
								cursorLeft += amount;
							}

							currentChar = nextChar;
						}

						DrawCharacter(type, currentChar, texture, cursorLeft, cursorTop, clip, color);
					}
				}

				cursorTop += type.LineHeight;
			}
		}
	}

	private int DrawCharacter(FontType type, char character, IVideoImage texture, int left, int top, in VideoRect clip, in VideoColor color)
	{
		if (type.Chars.TryGetValue(character, out var charInfo) || type.Chars.TryGetValue(' ', out charInfo))
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

	private static void LoadFontDefinition(FontType dst, string path)
	{
		dst.Chars.Clear();
		dst.Kernings.Clear();

		var fontScript = File.ReadAllText(path);
		var parsed     = ParseFontDefinition(fontScript);

		foreach (var element in parsed)
		{
			switch (element.Name)
			{
				case "info":
				case "common":
					ProcessCommonElement(dst, element);
					break;
				case "char":
					ProcessCharElement(dst, element);
					break;
				case "kerning":
					ProcessKerningElement(dst, element);
					break;
			}
		}
	}

	private static IEnumerable<ScriptElement> ParseFontDefinition(string source)
	{
		var whitespace = new[] { ' ', '\r', '\n', '\t', '\v' };
		var reader     = new StringReader(source);

		for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
		{
			var tokens = line.Split(whitespace, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			if (!tokens.Any())
			{
				continue;
			}

			yield return new ScriptElement
			{
				Name = tokens.First().ToLower(),
				Properties = tokens.Skip(1).Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries))
				                   .Where(x => x.Length == 2)
				                   .ToDictionary(x => x[0].ToLower(), x => x[1])
			};
		}
	}

	private static void ProcessCharElement(FontType dst, ScriptElement element)
	{
		if (element.Properties.TryGetValue("id", out var idScript) && int.TryParse(idScript, out var characterValue) &&
		    element.Properties.TryGetValue("x", out var leftScript) && int.TryParse(leftScript, out var leftValue) &&
		    element.Properties.TryGetValue("y", out var topScript) && int.TryParse(topScript, out var topValue) &&
		    element.Properties.TryGetValue("width", out var widthScript) && int.TryParse(widthScript, out var widthValue) &&
		    element.Properties.TryGetValue("height", out var heightScript) && int.TryParse(heightScript, out var heightValue))
		{
			var offsetX = 0;
			var offsetY = 0;
			var advance = (int)widthValue;

			if (element.Properties.TryGetValue("xoffset", out var offsetXScript) && int.TryParse(offsetXScript, out var offsetXValue))
			{
				offsetX = offsetXValue;
			}

			if (element.Properties.TryGetValue("yoffset", out var offsetYScript) && int.TryParse(offsetYScript, out var offsetYValue))
			{
				offsetY = offsetYValue;
			}

			if (element.Properties.TryGetValue("advance", out var advanceScript) && int.TryParse(advanceScript, out var advanceValue))
			{
				advance = advanceValue;
			}

			dst.Chars[(char)characterValue] = new FontChar
			{
				ImageRect  = new VideoRect { Left = leftValue, Top = topValue, Width = widthValue, Height = heightValue },
				OffsetLeft = offsetX,
				OffsetTop  = offsetY,
				Advance    = advance
			};
		}
	}

	private static void ProcessKerningElement(FontType dst, ScriptElement element)
	{
		if (element.Properties.TryGetValue("first", out var firstScript) && int.TryParse(firstScript, out var firstValue) &&
		    element.Properties.TryGetValue("second", out var secondScript) && int.TryParse(secondScript, out var secondValue) &&
		    element.Properties.TryGetValue("amount", out var amountScript) && int.TryParse(amountScript, out var amountValue))
		{
			dst.Kernings[new FontKerning((char)firstValue, (char)secondValue)] = amountValue;
		}
	}

	private static void ProcessCommonElement(FontType dst, ScriptElement element)
	{
		if (element.Properties.TryGetValue("lineheight", out var line) && int.TryParse(line, out var lineValue))
		{
			dst.LineHeight = lineValue;
		}
	}

	private class FontChar
	{
		public VideoRect ImageRect  { get; init; }
		public int       OffsetLeft { get; init; }
		public int       OffsetTop  { get; init; }
		public int       Advance    { get; init; }
	}

	private readonly struct FontKerning
	{
		private readonly char m_first;
		private readonly char m_second;

		public FontKerning(char first, char second)
		{
			m_first  = first;
			m_second = second;
		}

		public bool Equals(FontKerning other)
		{
			return m_first == other.m_first && m_second == other.m_second;
		}

		public override bool Equals(object? obj)
		{
			return obj is FontKerning other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(m_first, m_second);
		}

		public static bool operator ==(FontKerning left, FontKerning right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FontKerning left, FontKerning right)
		{
			return !left.Equals(right);
		}
	}

	private class ScriptElement
	{
		public string                              Name       { get; init; } = string.Empty;
		public IReadOnlyDictionary<string, string> Properties { get; init; } = null!;
	}

	private class FontType
	{
		public FontType()
		{
			Chars      = new Dictionary<char, FontChar>();
			Kernings   = new Dictionary<FontKerning, int>();
			LineHeight = 0;
		}

		public readonly Dictionary<char, FontChar>   Chars;
		public readonly Dictionary<FontKerning, int> Kernings;
		public          int                          LineHeight;
	}
}
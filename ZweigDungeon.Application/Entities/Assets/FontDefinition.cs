using System.Text;
using ZweigDungeon.Application.Entities.Assets.Font;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Assets;

public class FontDefinition
{
	public IReadOnlyDictionary<char, FontChar>   Chars      { get; init; } = null!;
	public IReadOnlyDictionary<FontKerning, int> Kernings   { get; init; } = null!;
	public int                                   LineHeight { get; init; } = 0;

	public string LayoutString(string text, int viewportWidth)
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
					var wordSize = MeasureString(word);
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

			return resultBuilder.ToString();
		}
	}

	public void Draw(IVideoImage texture, string text, int left, int top, in VideoRect clip, in VideoColor color)
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
							cursorLeft += DrawCharacter(currentChar, texture, cursorLeft, cursorTop, clip, color);

							if (Kernings.TryGetValue(kerning, out var amount))
							{
								cursorLeft += amount;
							}

							currentChar = nextChar;
						}

						DrawCharacter(currentChar, texture, cursorLeft, cursorTop, clip, color);
					}
				}

				cursorTop += LineHeight;
			}
		}
	}

	private int MeasureString(string word)
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
					wordSize += MeasureCharacter(currentChar);

					if (Kernings.TryGetValue(kerning, out var amount))
					{
						wordSize += amount;
					}

					currentChar = nextChar;
				}

				wordSize += MeasureCharacter(currentChar);
			}
		}

		return wordSize;
	}

	private int MeasureCharacter(char character)
	{
		if (Chars.TryGetValue(character, out var charInfo) || Chars.TryGetValue(' ', out charInfo))
		{
			return charInfo.Advance + charInfo.OffsetLeft;
		}

		return 0;
	}

	private int DrawCharacter(char character, IVideoImage texture, int left, int top, in VideoRect clip, in VideoColor color)
	{
		if (Chars.TryGetValue(character, out var charInfo) || Chars.TryGetValue(' ', out charInfo))
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
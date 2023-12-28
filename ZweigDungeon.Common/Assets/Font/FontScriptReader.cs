using ZweigDungeon.Common.Interfaces.Video;

namespace ZweigDungeon.Common.Assets.Font;

public class FontScriptReader
{
	public async Task<FontDefinition?> LoadContents(Stream stream, CancellationToken cancellationToken)
	{
		var definition = new FontDefinition();
		var characters = new List<FontCharacter>();
		var kernings   = new List<FontKerning>();
		var source     = await LoadSource(stream, cancellationToken).ConfigureAwait(false);
		var elements   = Parse(source);

		foreach (var element in elements)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			switch (element.Name)
			{
				case "info":
					ProcessInfoElement(element, definition);
					break;
				case "common":
					ProcessCommonElement(element, definition);
					break;
				case "page":
					ProcessPageElement(definition, element);
					break;
				case "char":
					ProcessCharElement(element, characters);
					break;
				case "kerning":
					ProcessKerningElement(element, kernings);
					break;
			}
		}

		if (!characters.Any() || string.IsNullOrEmpty(definition.ImageName) || string.IsNullOrEmpty(definition.FamilyName) ||
		    definition.LineHeight == 0 || definition.Size == 0)
		{
			return null;
		}

		definition.Characters = characters.ToArray();
		definition.Kernings   = kernings.ToArray();
		return definition;
	}

	private static void ProcessInfoElement(Element element, FontDefinition definition)
	{
		if (element.Properties.TryGetValue("face", out var family))
		{
			definition.FamilyName = family;
		}

		if (element.Properties.TryGetValue("size", out var size) && int.TryParse(size, out var sizeValue))
		{
			definition.Size = sizeValue;
		}

		if (element.Properties.TryGetValue("bold", out var bold) && int.TryParse(bold, out var boldValue))
		{
			definition.IsBold = boldValue != 0;
		}

		if (element.Properties.TryGetValue("italic", out var italic) && int.TryParse(italic, out var italicValue))
		{
			definition.IsItalic = italicValue != 0;
		}
	}

	private static void ProcessCommonElement(Element element, FontDefinition definition)
	{
		if (element.Properties.TryGetValue("lineheight", out var line) && int.TryParse(line, out var lineValue))
		{
			definition.LineHeight = lineValue;
		}

		if (element.Properties.TryGetValue("scaleW", out var width) && int.TryParse(width, out var widthValue))
		{
			definition.ImageWidth = widthValue;
		}

		if (element.Properties.TryGetValue("scaleH", out var height) && int.TryParse(height, out var heightValue))
		{
			definition.ImageHeight = heightValue;
		}
	}

	private static void ProcessPageElement(FontDefinition definition, Element element)
	{
		if (!string.IsNullOrEmpty(definition.ImageName))
		{
			throw new NotSupportedException("Font script containing multiple images are not supported.");
		}

		if (element.Properties.TryGetValue("file", out var path))
		{
			definition.ImageName = path;
		}
	}

	private static async Task<string> LoadSource(Stream stream, CancellationToken cancellationToken)
	{
		string source;
		using (var reader = new StreamReader(stream))
		{
			source = await reader.ReadToEndAsync(cancellationToken);
		}

		return source;
	}

	private static IEnumerable<Element> Parse(string source)
	{
		var whitespace = new char[] { ' ', '\r', '\n', '\t', '\v' };

		var reader = new StringReader(source);
		for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
		{
			var tokens = line.Split(whitespace, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			if (!tokens.Any())
			{
				continue;
			}

			yield return new Element
			{
				Name = tokens.First().ToLower(),
				Properties = tokens.Skip(1).Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries))
				                   .Where(x => x.Length == 2)
				                   .ToDictionary(x => x[0].ToLower(), x => x[1])
			};
		}
	}

	private static void ProcessCharElement(Element element, List<FontCharacter> characters)
	{
		if (element.Properties.TryGetValue("id", out var idScript) && int.TryParse(idScript, out var characterValue) &&
		    element.Properties.TryGetValue("x", out var leftScript) && uint.TryParse(leftScript, out var leftValue) &&
		    element.Properties.TryGetValue("y", out var topScript) && uint.TryParse(topScript, out var topValue) &&
		    element.Properties.TryGetValue("y", out var widthScript) && uint.TryParse(widthScript, out var widthValue) &&
		    element.Properties.TryGetValue("y", out var heightScript) && uint.TryParse(heightScript, out var heightValue))
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

			characters.Add(new FontCharacter
			{
				Character = (char)characterValue,
				Rect = new VideoRect
				{
					Left   = leftValue,
					Top    = topValue,
					Width  = widthValue,
					Height = heightValue
				},
				OffsetLeft = offsetX,
				OffsetTop  = offsetY,
				Advance    = advance
			});
		}
	}

	private static void ProcessKerningElement(Element element, List<FontKerning> kernings)
	{
		if (element.Properties.TryGetValue("first", out var firstScript) && int.TryParse(firstScript, out var firstValue) &&
		    element.Properties.TryGetValue("second", out var secondScript) && int.TryParse(secondScript, out var secondValue) &&
		    element.Properties.TryGetValue("amount", out var amountScript) && int.TryParse(amountScript, out var amountValue))
		{
			kernings.Add(new FontKerning
			{
				FirstCharacter  = (char)firstValue,
				SecondCharacter = (char)secondValue,
				Amount          = amountValue
			});
		}
	}

	private class Element
	{
		public string                              Name       { get; init; } = string.Empty;
		public IReadOnlyDictionary<string, string> Properties { get; init; } = null!;
	}
}
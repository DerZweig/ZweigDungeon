using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Utility.Concurrency;

namespace ZweigDungeon.Application.Services.Implementation;

public class FontRepository : IFontRepository
{
	private readonly IGlobalCancellation       m_cancellation;
	private readonly ExclusiveTaskFactory      m_synchronization;
	private readonly Dictionary<string, Entry> m_fonts;

	public FontRepository(IGlobalCancellation cancellation)
	{
		m_cancellation    = cancellation;
		m_synchronization = new ExclusiveTaskFactory();
		m_fonts           = new Dictionary<string, Entry>();
	}

	public Task<Font> LoadFont(string name) => m_synchronization.Invoke(async () =>
	{
		var normalized = name.Trim().ToLower();
		if (m_fonts.TryGetValue(normalized, out var entry))
		{
			if (entry.Pending != null)
			{
				await entry.Pending;
			}

			return entry.Font ?? throw new NullReferenceException();
		}

		entry = new Entry();
		m_fonts.Add(normalized, entry);

		var worker = LoadFontDefinition(name);
		entry.Pending = worker;
		entry.Font    = await worker;
		entry.Pending = null;
		return entry.Font ?? throw new NullReferenceException();
	}, m_cancellation.Token);

	private async Task<Font> LoadFontDefinition(string name)
	{
		var path       = Path.Combine("Data", name.Trim());
		var fntPath    = Path.ChangeExtension(path, ".fnt");
		var fontScript = await File.ReadAllTextAsync(fntPath, m_cancellation.Token);
		var parsed     = ParseFontDefinition(fontScript);

		var lineHeight = 0;
		var characters = new Dictionary<char, FontChar>();
		var kernings   = new Dictionary<FontKerning, int>();

		foreach (var element in parsed)
		{
			switch (element.Name)
			{
				case "info":
				case "common":
					if (element.Properties.TryGetValue("lineheight", out var line) && int.TryParse(line, out var lineValue))
					{
						lineHeight = lineValue;
					}

					break;
				case "char":
					if (element.Properties.TryGetValue("id", out var idScript) && int.TryParse(idScript, out var characterValue) &&
					    element.Properties.TryGetValue("x", out var leftScript) && int.TryParse(leftScript, out var leftValue) &&
					    element.Properties.TryGetValue("y", out var topScript) && int.TryParse(topScript, out var topValue) &&
					    element.Properties.TryGetValue("width", out var widthScript) && int.TryParse(widthScript, out var widthValue) &&
					    element.Properties.TryGetValue("height", out var heightScript) && int.TryParse(heightScript, out var heightValue))
					{
						var offsetX = 0;
						var offsetY = 0;
						var advance = widthValue;

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

						characters[(char)characterValue] = new FontChar
						{
							ImageRect  = new VideoRect { Left = leftValue, Top = topValue, Width = widthValue, Height = heightValue },
							OffsetLeft = offsetX,
							OffsetTop  = offsetY,
							Advance    = advance
						};
					}

					break;
				case "kerning":
					if (element.Properties.TryGetValue("first", out var firstScript) && int.TryParse(firstScript, out var firstValue) &&
					    element.Properties.TryGetValue("second", out var secondScript) && int.TryParse(secondScript, out var secondValue) &&
					    element.Properties.TryGetValue("amount", out var amountScript) && int.TryParse(amountScript, out var amountValue))
					{
						kernings[new FontKerning((char)firstValue, (char)secondValue)] = amountValue;
					}

					break;
			}
		}

		return new Font
		{
			Chars      = characters,
			Kernings   = kernings,
			LineHeight = lineHeight
		};
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

	private class Entry
	{
		public Task? Pending { get; set; }
		public Font? Font    { get; set; }
	}

	private class ScriptElement
	{
		public string                              Name       { get; init; } = string.Empty;
		public IReadOnlyDictionary<string, string> Properties { get; init; } = null!;
	}
}
using System.Xml.Linq;
using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Utility.Concurrency;

namespace ZweigDungeon.Application.Services.Implementation;

public class PanelRepository : IPanelRepository
{
	private readonly IGlobalCancellation       m_cancellation;
	private readonly ExclusiveTaskFactory      m_synchronization;
	private readonly Dictionary<string, Entry> m_panels;

	public PanelRepository(IGlobalCancellation cancellation)
	{
		m_cancellation    = cancellation;
		m_synchronization = new ExclusiveTaskFactory();
		m_panels          = new Dictionary<string, Entry>();
	}

	public Task<Panel> LoadPanel(string path, string name) => m_synchronization.Invoke(async () =>
	{
		var normalized = path.Trim().ToLower();
		if (m_panels.TryGetValue(normalized, out var entry))
		{
			if (entry.Pending != null)
			{
				await entry.Pending;
			}

			if (entry.Panels?.TryGetValue(name, out var panel) == true)
			{
				return panel;
			}

			throw new NullReferenceException();
		}

		entry = new Entry();
		m_panels.Add(normalized, entry);
		try
		{
			var worker = LoadPanelDefinitions(path);
			entry.Pending = worker;
			entry.Panels  = await worker;

			if (entry.Panels?.TryGetValue(name, out var panel) == true)
			{
				return panel;
			}

			throw new NullReferenceException();
		}
		finally
		{
			entry.Pending = null;
		}
	}, m_cancellation.Token);

	private async Task<Dictionary<string, Panel>> LoadPanelDefinitions(string path)
	{
		var dataPath    = Path.Combine("Data", path.Trim());
		var xmlPath     = Path.ChangeExtension(dataPath, ".xml");
		var panelScript = await File.ReadAllTextAsync(xmlPath, m_cancellation.Token);
		var document    = XDocument.Parse(panelScript);
		var root        = document.Root;
		if (root == null || !string.Equals(root.Name.LocalName, "panels", StringComparison.OrdinalIgnoreCase))
		{
			throw new FileLoadException("Panel definition does not contain valid root node.");
		}

		var results = new Dictionary<string, Panel>();
		var items   = root.Elements();
		foreach (var node in items)
		{
			if (string.Equals(node.Name.LocalName, "panel", StringComparison.OrdinalIgnoreCase))
			{
				var name = ParseStringProperty(node, "name");
				if (string.IsNullOrWhiteSpace(name))
				{
					continue;
				}

				var panel = new Panel();
				results[name] = panel;

				var borderText = ParseStringProperty(node, "border");
				var rectText   = ParseStringProperty(node, "rect");

				if (string.IsNullOrWhiteSpace(borderText))
				{
					var borderNode = node.Elements().SingleOrDefault(x => string.Equals(x.Name.LocalName, "border", StringComparison.OrdinalIgnoreCase));
					if (borderNode != null)
					{
						panel.BorderLeft   = ParseIntegerProperty(borderNode, "left") ?? 0;
						panel.BorderTop    = ParseIntegerProperty(borderNode, "top") ?? 0;
						panel.BorderRight  = ParseIntegerProperty(borderNode, "right") ?? 0;
						panel.BorderBottom = ParseIntegerProperty(borderNode, "bottom") ?? 0;
					}
				}
				else
				{
					var borderValues = borderText.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
					switch (borderValues.Length)
					{
						case >= 4 when
							int.TryParse(borderValues[0], out var left) &&
							int.TryParse(borderValues[1], out var top) &&
							int.TryParse(borderValues[1], out var right) &&
							int.TryParse(borderValues[1], out var bottom):
							panel.BorderLeft   = left;
							panel.BorderTop    = top;
							panel.BorderRight  = right;
							panel.BorderBottom = bottom;
							break;
						case >= 2 when
							int.TryParse(borderValues[0], out var leftRight) &&
							int.TryParse(borderValues[1], out var topBottom):
							panel.BorderLeft   = leftRight;
							panel.BorderRight  = leftRight;
							panel.BorderTop    = topBottom;
							panel.BorderBottom = topBottom;
							break;
						case 1 when int.TryParse(borderValues[0], out var borderSize):
							panel.BorderLeft   = borderSize;
							panel.BorderTop    = borderSize;
							panel.BorderRight  = borderSize;
							panel.BorderBottom = borderSize;
							break;
					}
				}

				if (string.IsNullOrWhiteSpace(rectText))
				{
					var rectNode = node.Elements().SingleOrDefault(x => string.Equals(x.Name.LocalName, "source", StringComparison.OrdinalIgnoreCase));
					if (rectNode != null)
					{
						panel.ImageRect = new VideoRect
						{
							Left   = ParseIntegerProperty(rectNode, "left") ?? 0,
							Top    = ParseIntegerProperty(rectNode, "top") ?? 0,
							Width  = ParseIntegerProperty(rectNode, "width") ?? 0,
							Height = ParseIntegerProperty(rectNode, "height") ?? 0
						};
					}
				}
				else
				{
					var rectValues = rectText.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
					if (rectValues.Length == 4 &&
					    int.TryParse(rectValues[0], out var left) &&
					    int.TryParse(rectValues[1], out var top) &&
					    int.TryParse(rectValues[2], out var width) &&
					    int.TryParse(rectValues[3], out var height))
					{
						panel.ImageRect = new VideoRect
						{
							Left   = left,
							Top    = top,
							Width  = width,
							Height = height
						};
					}
				}
			}
		}

		return results;
	}

	private static string? ParseStringProperty(XElement node, string name)
	{
		var attributes = node.Attributes();
		foreach (var attribute in attributes)
		{
			if (string.Equals(attribute.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
			{
				return attribute.Value;
			}
		}

		var children = node.Elements();
		foreach (var child in children)
		{
			if (string.Equals(child.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
			{
				return child.Value;
			}
		}

		return null;
	}

	private static int? ParseIntegerProperty(XElement node, string name)
	{
		var text = ParseStringProperty(node, name);
		if (!string.IsNullOrEmpty(text) && int.TryParse(text, out var value))
		{
			return value;
		}

		return null;
	}

	private class Entry
	{
		public Task?                      Pending { get; set; }
		public Dictionary<string, Panel>? Panels  { get; set; }
	}
}
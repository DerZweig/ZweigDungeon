using System.Xml.Linq;
using ZweigDungeon.Application.Entities.Assets;
using ZweigDungeon.Application.Entities.Assets.Menu.Constants;
using ZweigDungeon.Application.Entities.Assets.Menu.Controls;
using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Utility.Concurrency;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuRepository : IMenuRepository
{
	private readonly IGlobalCancellation       m_cancellation;
	private readonly ExclusiveTaskFactory      m_synchronization;
	private readonly Dictionary<string, Entry> m_menus;

	public MenuRepository(IGlobalCancellation cancellation)
	{
		m_cancellation    = cancellation;
		m_synchronization = new ExclusiveTaskFactory();
		m_menus           = new Dictionary<string, Entry>();
	}

	public Task<MenuDefinition> LoadMenu(string name) => m_synchronization.Invoke(async () =>
	{
		var normalized = name.Trim().ToLower();
		if (m_menus.TryGetValue(normalized, out var entry))
		{
			if (entry.Pending != null)
			{
				await entry.Pending;
			}

			return entry.Menu ?? throw new NullReferenceException();
		}

		entry = new Entry();
		m_menus.Add(normalized, entry);

		var worker = LoadMenuDefinition(name, m_cancellation.Token);
		entry.Pending = worker;
		entry.Menu    = await worker;
		entry.Pending = null;
		return entry.Menu ?? throw new NullReferenceException();
	}, m_cancellation.Token);

	private async Task<MenuDefinition> LoadMenuDefinition(string name, CancellationToken cancellationToken)
	{
		var path     = Path.Combine("Data", name.Trim());
		var xmlPath  = Path.ChangeExtension(path, ".xml");
		var script   = await File.ReadAllTextAsync(xmlPath, m_cancellation.Token);
		var document = XDocument.Parse(script);
		var root     = document.Root;

		if (root == null || !string.Equals(root.Name.LocalName, "panel", StringComparison.OrdinalIgnoreCase))
		{
			throw new FileLoadException("Menu definition does not contain valid root.");
		}

		var menu    = new MenuDefinition();
		var pending = new Queue<(BasicControl control, XElement node)>();
		pending.Enqueue(new(menu.Panel, root));

		while (pending.TryDequeue(out var pair))
		{
			cancellationToken.ThrowIfCancellationRequested();

			var control = pair.control;
			var node    = pair.node;

			ParseCommonProperties(control, node);

			switch (control)
			{
				case ButtonControl button:
					ParseButtonProperties(button, node);
					break;
				case ImageControl image:
					ParseImageProperties(image, node);
					break;
				case InputControl input:
					ParseInputProperties(input, node);
					break;
				case TextControl text:
					text.FontSize = ParseEnumProperty(node, nameof(TextControl.FontSize), FontSize.Small);
					text.Text     = ParseStringProperty(node, nameof(TextControl.Text)) ?? node.Value;
					break;
				case PanelControl panel:
					panel.Layout = ParseEnumProperty(node, nameof(PanelControl.Layout), PanelLayout.None);
					foreach (var childNode in node.Elements())
					{
						var childControl = ConstructPanelChild(childNode);
						if (childControl == null)
						{
							continue;
						}

						panel.Children.Add(childControl);
						pending.Enqueue(new(childControl, childNode));
					}

					break;
			}
		}

		return menu;
	}

	private static BasicControl? ConstructPanelChild(XElement node)
	{
		var childName = node.Name.LocalName.ToLower();
		return childName switch
		       {
			       "button" => new ButtonControl(),
			       "image" => new ImageControl(),
			       "input" => new InputControl(),
			       "text" => new TextControl(),
			       "panel" => new PanelControl(),
			       _ => null //possible attribute
		       };
	}

	private static BasicControl? ConstructButtonChild(XElement node)
	{
		var childName = node.Name.LocalName.ToLower();
		return childName switch
		       {
			       "image" => new ImageControl(),
			       "text" => new TextControl(),
			       "panel" => new PanelControl(),
			       _ => null //possible attribute
		       };
	}

	private static void ParseCommonProperties(BasicControl control, XElement node)
	{
		const string widthAttributeName  = "width";
		const string heightAttributeName = "height";
		const string marginAttributeName = "margin";

		control.HorizontalAlignment = ParseEnumProperty(node, nameof(BasicControl.HorizontalAlignment), HorizontalAlignment.Left);
		control.VerticalAlignment   = ParseEnumProperty(node, nameof(BasicControl.VerticalAlignment), VerticalAlignment.Top);

		var width      = ParseIntegerProperty(node, widthAttributeName);
		var height     = ParseIntegerProperty(node, heightAttributeName);
		var marginNode = node.Elements().SingleOrDefault(x => string.Equals(x.Name.LocalName, marginAttributeName));

		if (width != null)
		{
			control.MinimumWidth = width;
			control.MaximumWidth = width;
		}
		else
		{
			control.MinimumWidth = ParseIntegerProperty(node, nameof(BasicControl.MinimumWidth));
			control.MaximumWidth = ParseIntegerProperty(node, nameof(BasicControl.MaximumWidth));
		}

		if (height != null)
		{
			control.MinimumHeight = height;
			control.MaximumHeight = height;
		}
		else
		{
			control.MinimumHeight = ParseIntegerProperty(node, nameof(BasicControl.MinimumHeight));
			control.MaximumHeight = ParseIntegerProperty(node, nameof(BasicControl.MaximumHeight));
		}

		if (marginNode != null)
		{
			control.MarginLeft   = ParseIntegerProperty(marginNode, "left");
			control.MarginTop    = ParseIntegerProperty(marginNode, "top");
			control.MarginRight  = ParseIntegerProperty(marginNode, "right");
			control.MarginBottom = ParseIntegerProperty(marginNode, "bottom");
		}

		if (control.MarginLeft == null && control.MarginTop == null && control.MarginRight == null && control.MarginBottom == null)
		{
			var marginText  = ParseStringProperty(node, marginAttributeName);
			var marginParts = marginText?.Split(",") ?? Array.Empty<string>();
			if (marginParts.Length == 1 && int.TryParse(marginParts[0], out var marginAll))
			{
				control.MarginLeft   = marginAll;
				control.MarginTop    = marginAll;
				control.MarginRight  = marginAll;
				control.MarginBottom = marginAll;
			}
			else if (marginParts.Length == 2 &&
			         int.TryParse(marginParts[0], out var marginWidth) &&
			         int.TryParse(marginParts[1], out var marginHeight))
			{
				control.MarginLeft   = marginWidth;
				control.MarginRight  = marginWidth;
				control.MarginTop    = marginHeight;
				control.MarginBottom = marginHeight;
			}
			else if (marginParts.Length == 4 &&
			         int.TryParse(marginParts[0], out var marginLeft) &&
			         int.TryParse(marginParts[1], out var marginTop) &&
			         int.TryParse(marginParts[2], out var marginRight) &&
			         int.TryParse(marginParts[3], out var marginBottom))
			{
				control.MarginLeft   = marginLeft;
				control.MarginTop    = marginTop;
				control.MarginRight  = marginRight;
				control.MarginBottom = marginBottom;
			}
		}

		if (control.MarginLeft == null && control.MarginTop == null && control.MarginRight == null && control.MarginBottom == null)
		{
			control.MarginLeft   = ParseIntegerProperty(node, $"{marginAttributeName}.left");
			control.MarginTop    = ParseIntegerProperty(node, $"{marginAttributeName}.top");
			control.MarginRight  = ParseIntegerProperty(node, $"{marginAttributeName}.right");
			control.MarginBottom = ParseIntegerProperty(node, $"{marginAttributeName}.bottom");
		}
	}

	private static void ParseInputProperties(InputControl input, XElement node)
	{
		input.Type          = ParseEnumProperty(node, nameof(InputControl.Type), InputType.String);
		input.DefaultValue  = ParseStringProperty(node, nameof(InputControl.DefaultValue));
		input.MinimumValue  = ParseIntegerProperty(node, nameof(InputControl.MinimumValue));
		input.MaximumValue  = ParseIntegerProperty(node, nameof(InputControl.MaximumValue));
		input.MaximumLength = ParseIntegerProperty(node, nameof(InputControl.MaximumLength));
	}

	private static void ParseButtonProperties(ButtonControl button, XElement node)
	{
		var pending = new Queue<(BasicControl control, XElement node)>();
		foreach (var childNode in node.Elements())
		{
			var childControl = ConstructButtonChild(childNode);
			if (childControl != null)
			{
				button.Children.Add(childControl);
				pending.Enqueue(new(childControl, childNode));
			}
		}

		if (button.Children.Any())
		{
			while (pending.TryDequeue(out var pair))
			{
				var childControl = pair.control;
				var childNode    = pair.node;
				ParseCommonProperties(childControl, childNode);

				switch (childControl)
				{
					case ImageControl image:
						ParseImageProperties(image, childNode);
						break;
					case TextControl text:
						text.FontSize = ParseEnumProperty(childNode, nameof(TextControl.FontSize), FontSize.Small);
						text.Text     = ParseStringProperty(childNode, nameof(TextControl.Text)) ?? childNode.Value;
						break;
					case PanelControl panel:
						panel.Layout = ParseEnumProperty(childNode, nameof(PanelControl.Layout), PanelLayout.None);
						foreach (var panelChildNode in childNode.Elements())
						{
							var panelChildControl = ConstructPanelChild(panelChildNode);
							if (panelChildControl == null)
							{
								continue;
							}

							pending.Enqueue(new(panelChildControl, panelChildNode));
						}

						break;
				}
			}
		}
		else
		{
			var text = node.Value;
			if (!string.IsNullOrEmpty(text))
			{
				button.Children.Add(new TextControl
				{
					HorizontalAlignment = HorizontalAlignment.Center,
					FontSize            = FontSize.Medium,
					Text                = text
				});
			}
		}
	}

	private static void ParseImageProperties(ImageControl image, XElement node)
	{
		const string rectAttributeName = "SubImage";
		image.Path = ParseStringProperty(node, nameof(ImageControl.Path)) ?? string.Empty;

		var rectNode = node.Elements().SingleOrDefault(x => string.Equals(x.Name.LocalName, rectAttributeName));
		if (rectNode != null)
		{
			image.SubImageLeft   = ParseIntegerProperty(rectNode, "left");
			image.SubImageTop    = ParseIntegerProperty(rectNode, "top");
			image.SubImageWidth  = ParseIntegerProperty(rectNode, "width");
			image.SubImageHeight = ParseIntegerProperty(rectNode, "height");
		}

		if (image.SubImageLeft == null && image.SubImageTop == null && image.SubImageWidth == null && image.SubImageHeight == null)
		{
			image.SubImageLeft   = ParseIntegerProperty(node, $"{rectAttributeName}.left");
			image.SubImageTop    = ParseIntegerProperty(node, $"{rectAttributeName}.top");
			image.SubImageWidth  = ParseIntegerProperty(node, $"{rectAttributeName}.width");
			image.SubImageHeight = ParseIntegerProperty(node, $"{rectAttributeName}.height");
		}
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

	private static TEnum ParseEnumProperty<TEnum>(XElement node, string name, TEnum fallback) where TEnum : struct
	{
		var text = ParseStringProperty(node, name);
		if (!string.IsNullOrEmpty(text) && Enum.TryParse<TEnum>(text, out var value))
		{
			return value;
		}

		return fallback;
	}

	private class Entry
	{
		public Task?           Pending;
		public MenuDefinition? Menu;
	}
}
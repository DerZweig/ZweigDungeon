using ZweigDungeon.Application.Gui.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Video.Structures;

namespace ZweigDungeon.Application.Gui.Controls;

public class TextControl : BasicControl
{
	public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Left;
	public FontSize            TextFont      { get; set; } = FontSize.Medium;
	public string              Text          { get; set; } = string.Empty;
	public VideoColor          TextColor     { get; set; }
}
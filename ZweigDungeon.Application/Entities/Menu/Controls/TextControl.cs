using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class TextControl : BasicControl
{
	public HorizontalAlignment TextAlignment { get; set; } = HorizontalAlignment.Left;
	public FontSize            TextFont      { get; set; } = FontSize.Medium;
	public string              Text          { get; set; } = string.Empty;
	public VideoColor          TextColor     { get; set; }
}
using ZweigDungeon.Application.Entities.Menu.Constants;
using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class ButtonControl : BasicControl
{
	public FontSize   LabelFont  { get; set; } = FontSize.Medium;
	public string     Label      { get; set; } = string.Empty;
	public VideoColor LabelColor { get; set; }
	public Action?    OnClick    { get; set; }
}
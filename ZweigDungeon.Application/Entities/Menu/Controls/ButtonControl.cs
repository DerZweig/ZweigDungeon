using System;
using ZweigEngine.Common.Assets.Font;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class ButtonControl : BasicControl
{
	public FontAsset Font    { get; set; } = FontAsset.Empty;
	public string    Label   { get; set; } = string.Empty;
	public Action?   OnClick { get; set; }
}
using System;

namespace ZweigDungeon.Application.Entities.Menu.Controls;

public class ToggleControl : BasicControl
{
	public bool    IsChecked { get; set; }
	public Action? OnChange  { get; set; }
}
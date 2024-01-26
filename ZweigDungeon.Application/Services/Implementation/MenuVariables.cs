using ZweigDungeon.Application.Gui;
using ZweigDungeon.Application.Gui.Menu;
using ZweigDungeon.Application.Services.Interfaces;

namespace ZweigDungeon.Application.Services.Implementation;

public class MenuVariables : IMenuVariables
{
	public MenuVariables()
	{
		ActiveMenu = new StartupMenu(); //initial state of menus
	}

	public BasicMenu?  ActiveMenu     { get; set; }
	public BasicPopup? ActiveDialog   { get; set; }
	public BasicPopup? ActivePopup    { get; set; } 
	public BasicPanel? LeftSidePanel  { get; set; }
	public BasicPanel? RightSidePanel { get; set; }
	public BasicPanel? BottomPanel    { get; set; }
}
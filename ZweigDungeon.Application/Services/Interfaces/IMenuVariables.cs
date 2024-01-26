using ZweigDungeon.Application.Gui;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IMenuVariables
{
	BasicMenu?  ActiveMenu     { get; set; }
	BasicPopup? ActiveDialog   { get; set; }
	BasicPopup? ActivePopup    { get; set; }
	BasicPanel? LeftSidePanel  { get; set; }
	BasicPanel? RightSidePanel { get; set; }
	BasicPanel? BottomPanel    { get; set; }
}
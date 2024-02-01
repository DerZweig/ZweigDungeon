using ZweigEngine.Common.Services.Video.Structures;

namespace ZweigDungeon.Application.Gui;

public abstract class BasicPanel
{
	public abstract void UpdateLayout(in VideoRect viewport);
}
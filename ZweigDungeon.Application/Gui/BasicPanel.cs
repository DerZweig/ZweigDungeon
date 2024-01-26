using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Gui;

public abstract class BasicPanel
{
	public abstract void UpdateLayout(in VideoRect viewport);
}
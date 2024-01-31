using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Video.Structures;

namespace ZweigDungeon.Application.Gui;

public abstract class BasicPanel
{
	public abstract void UpdateLayout(in VideoRect viewport);
}
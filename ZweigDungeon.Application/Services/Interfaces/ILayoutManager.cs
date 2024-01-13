using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface ILayoutManager
{
	public void Update(in VideoRect viewport);
}
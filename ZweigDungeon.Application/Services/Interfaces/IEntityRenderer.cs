using ZweigEngine.Common.Services.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IEntityRenderer
{
	void Draw(in VideoRect viewport);
}
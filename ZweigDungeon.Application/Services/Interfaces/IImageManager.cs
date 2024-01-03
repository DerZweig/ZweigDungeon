using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IImageManager
{
	public Task Load(string name);
	public Task Bind(string name, Action<IVideoImage> work);
}
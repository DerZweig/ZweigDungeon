using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Manager.Interfaces;

public interface IImageManager
{
	public void Load(string name);
	public void Bind(string name, Action<IVideoImage> work);
}
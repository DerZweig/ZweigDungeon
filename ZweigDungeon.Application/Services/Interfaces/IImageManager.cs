using ZweigEngine.Image;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IImageManager
{
	Task<IImageInfo> LoadImage(string name);
}
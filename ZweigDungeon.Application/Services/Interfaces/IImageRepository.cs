using ZweigEngine.Image;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IImageRepository
{
	Task<IImageInfo> LoadImage(string name);
}
using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Image;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IImageRepository
{
	Task<Image> LoadImage(string name);
}
using ZweigDungeon.Application.Entities.Assets;
using ZweigEngine.Common.Interfaces.Video;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface ITextureManager
{
	public void Clear();
    public void Upload(Image image);
	public void Bind(Image image, Action<IVideoImage> work);
}
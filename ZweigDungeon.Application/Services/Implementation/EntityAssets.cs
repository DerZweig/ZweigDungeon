using ZweigDungeon.Application.Services.Interfaces;
using ZweigEngine.Common.Assets.Image;
using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Repositories;

namespace ZweigDungeon.Application.Services.Implementation;

public class EntityAssets : IEntityAssets, IDisposable
{
	private readonly IPlatformSynchronization m_synchronization;
	private readonly IGlobalCancellation      m_cancellation;
	private readonly IPlatformWindow          m_window;
	private readonly ImageRepository          m_images;
	private readonly TextureRepository        m_textures;
	private          ImageAsset               m_actors;

	public EntityAssets(IPlatformSynchronization synchronization, IGlobalCancellation cancellation, IPlatformWindow window,
	                    ImageRepository images, TextureRepository textures)
	{
		m_synchronization = synchronization;
		m_cancellation    = cancellation;
		m_window          = window;
		m_images          = images;
		m_textures        = textures;
		m_actors          = ImageAsset.Empty;

		m_window.OnCreated += HandleWindowCreated;
	}

	private void ReleaseUnmanagedResources()
	{
		m_window.OnCreated -= HandleWindowCreated;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~EntityAssets()
	{
		ReleaseUnmanagedResources();
	}

	private void HandleWindowCreated(IPlatformWindow window)
	{
		var cancellationToken = m_cancellation.Token;
		m_synchronization.Invoke(async () =>
		{
			m_actors = await m_images.LoadForGlobal("Char/character", cancellationToken);
			await m_textures.LoadForGlobal(m_actors, cancellationToken);
		}, cancellationToken);
	}

	public ImageAsset GetActorImage()
	{
		return m_actors;
	}
}
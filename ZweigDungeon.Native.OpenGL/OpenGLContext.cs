using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Native.OpenGL.Managers;

namespace ZweigDungeon.Native.OpenGL;

public sealed class OpenGLContext : IDisposable, IVideoContext, IVideoDeviceListener
{
	private readonly IPlatformVideo m_video;
	private readonly IDisposable    m_deviceSubscription;

	private OpenGLStateManager?   m_stateManager;
	private OpenGLModelManager?   m_arrayManager;
	private OpenGLTextureManager? m_textureManager;
	private OpenGLShaderManager?  m_shaderManager;

	public OpenGLContext(IPlatformVideo video, MessageBus messageBus)
	{
		m_video              = video;
		m_deviceSubscription = messageBus.Subscribe<IVideoDeviceListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_deviceSubscription.Dispose();
		m_stateManager?.Dispose();
		m_arrayManager?.Dispose();
		m_textureManager?.Dispose();
		m_shaderManager?.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLContext()
	{
		ReleaseUnmanagedResources();
	}

	public void VideoDeviceActivated(IPlatformVideo video)
	{
		if (m_video != video)
		{
			return;
		}

		var loader = (ICustomFunctionLoader)m_video;
		m_stateManager   = new OpenGLStateManager(loader);
		m_arrayManager   = new OpenGLModelManager(loader);
		m_textureManager = new OpenGLTextureManager(loader);
		m_shaderManager  = new OpenGLShaderManager(loader);
	}

	public void VideoDeviceDeactivating(IPlatformVideo video)
	{
		if (m_video != video)
		{
			return;
		}

		m_stateManager?.Dispose();
		m_arrayManager?.Dispose();
		m_textureManager?.Dispose();
		m_shaderManager?.Dispose();

		m_stateManager   = null;
		m_arrayManager   = null;
		m_textureManager = null;
		m_shaderManager  = null;
	}
}
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

	private OpenGLStateBackend?   m_stateBackend;
	private OpenGLModelBackend?   m_modelBackend;
	private OpenGLTextureBackend? m_textureBackend;
	private OpenGLShaderBackend?  m_shaderBackend;

	public OpenGLContext(IPlatformVideo video, MessageBus messageBus)
	{
		m_video              = video;
		m_deviceSubscription = messageBus.Subscribe<IVideoDeviceListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_deviceSubscription.Dispose();
		m_stateBackend?.Dispose();
		m_modelBackend?.Dispose();
		m_textureBackend?.Dispose();
		m_shaderBackend?.Dispose();
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
		m_stateBackend   = new OpenGLStateBackend(loader);
		m_modelBackend   = new OpenGLModelBackend(loader);
		m_textureBackend = new OpenGLTextureBackend(loader);
		m_shaderBackend  = new OpenGLShaderBackend(loader);
	}

	public void VideoDeviceDeactivating(IPlatformVideo video)
	{
		if (m_video != video)
		{
			return;
		}

		m_stateBackend?.Dispose();
		m_modelBackend?.Dispose();
		m_textureBackend?.Dispose();
		m_shaderBackend?.Dispose();

		m_stateBackend   = null;
		m_modelBackend   = null;
		m_textureBackend = null;
		m_shaderBackend  = null;
	}
}
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Common.Services.Video;
using ZweigDungeon.Native.OpenGL.Backend;

namespace ZweigDungeon.Native.OpenGL;

public sealed class OpenGLContext : VideoContext, IDisposable, IVideoDeviceListener
{
	private readonly IPlatformVideo m_video;
	private readonly IDisposable    m_deviceSubscription;

	private OpenGLStateBackend?       m_stateBackend;
	private OpenGLArrayBackend?       m_arrayBackend;
	private OpenGLTextureBackend?     m_textureBackend;
	private OpenGLShaderBackend?      m_shaderBackend;
	private OpenGLQueryBackend?       m_queryBackend;
	private OpenGLFrameBufferBackend? m_frameBufferBackend;
	

	public OpenGLContext(IPlatformVideo video, MessageBus messageBus)
	{
		m_video              = video;
		m_deviceSubscription = messageBus.Subscribe<IVideoDeviceListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_deviceSubscription.Dispose();
		m_stateBackend?.Dispose();
		m_arrayBackend?.Dispose();
		m_textureBackend?.Dispose();
		m_shaderBackend?.Dispose();
		m_queryBackend?.Dispose();
		m_frameBufferBackend?.Dispose();
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
		m_stateBackend       = new OpenGLStateBackend(loader);
		m_arrayBackend       = new OpenGLArrayBackend(loader);
		m_textureBackend     = new OpenGLTextureBackend(loader);
		m_shaderBackend      = new OpenGLShaderBackend(loader);
		m_queryBackend       = new OpenGLQueryBackend(loader);
		m_frameBufferBackend = new OpenGLFrameBufferBackend(loader);
	}

	public void VideoDeviceDeactivating(IPlatformVideo video)
	{
		if (m_video != video)
		{
			return;
		}

		m_stateBackend?.Dispose();
		m_arrayBackend?.Dispose();
		m_textureBackend?.Dispose();
		m_shaderBackend?.Dispose();
		m_queryBackend?.Dispose();
		m_frameBufferBackend?.Dispose();

		m_stateBackend       = null;
		m_arrayBackend       = null;
		m_textureBackend     = null;
		m_shaderBackend      = null;
		m_queryBackend       = null;
		m_frameBufferBackend = null;
	}
}
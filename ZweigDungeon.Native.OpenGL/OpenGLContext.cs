using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Common.Services.Video;
using ZweigDungeon.Common.Services.Video.Descriptors;
using ZweigDungeon.Common.Services.Video.Resources;
using ZweigDungeon.Native.OpenGL.Backend;
using ZweigDungeon.Native.OpenGL.Resources;

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

	public override bool CreateShader(in VideoPixelShaderDescription desc, out VideoShader shader)
	{
		throw new NotImplementedException();
	}
	public override bool CreateRenderTarget(in VideoRenderTargetDescription desc, out VideoRenderTarget renderTarget)
	{
		throw new NotImplementedException();
	}

	public override bool CreateTexture2D(in VideoTexture2DDescription desc, out VideoTexture2D texture2D)
	{
		throw new NotImplementedException();
	}

	public override bool CreateVertexBuffer(in VideoVertexBufferDescription desc, out VideoVertexBuffer buffer)
	{
		throw new NotImplementedException();
	}

	public override void Delete(AbstractVideoResource resource)
	{
		switch (resource)
		{
			case OpenGLShader shader:
				break;
			case OpenGLTexture2D texture2D:
				break;
			case OpenGLVertexBuffer vertexBuffer:
				break;
			case OpenGLRenderTarget renderTarget:
				break;
			default:
				throw new InvalidOperationException($"{resource.GetType().Name} is not a valid resource of {nameof(OpenGLContext)}.");
		}
	}
}
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal class OpenGLFrameBufferManager : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnGenFramebuffersDelegate         glGenFramebuffers;
	private readonly PfnDeleteFramebuffersDelegate      glDeleteFramebuffers;
	private readonly PfnBindFramebufferDelegate         glBindFramebuffer;
	private readonly PfnCheckFramebufferStatusDelegate  glCheckFramebufferStatus;
	private readonly PfnFramebufferTexture2DDelegate    glFramebufferTexture2D;
	private readonly PfnFramebufferRenderbufferDelegate glFramebufferRenderbuffer;
	private readonly PfnGenRenderbuffersDelegate        glGenRenderbuffers;
	private readonly PfnDeleteRenderbuffersDelegate     glDeleteRenderbuffers;
	private readonly PfnBindRenderbufferDelegate        glBindRenderbuffer;
	private readonly PfnRenderbufferStorageDelegate     glRenderbufferStorage;
	// ReSharper restore InconsistentNaming

	private readonly Dictionary<OpenGLFrameBufferHandle, uint>  m_frameBuffers;
	private readonly Dictionary<OpenGLRenderBufferHandle, uint> m_renderBuffers;

	public OpenGLFrameBufferManager(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glGenFramebuffers), out glGenFramebuffers);
		loader.LoadFunction(nameof(glDeleteFramebuffers), out glDeleteFramebuffers);
		loader.LoadFunction(nameof(glBindFramebuffer), out glBindFramebuffer);
		loader.LoadFunction(nameof(glCheckFramebufferStatus), out glCheckFramebufferStatus);
		loader.LoadFunction(nameof(glFramebufferTexture2D), out glFramebufferTexture2D);
		loader.LoadFunction(nameof(glFramebufferRenderbuffer), out glFramebufferRenderbuffer);
		loader.LoadFunction(nameof(glGenRenderbuffers), out glGenRenderbuffers);
		loader.LoadFunction(nameof(glDeleteRenderbuffers), out glDeleteRenderbuffers);
		loader.LoadFunction(nameof(glBindRenderbuffer), out glBindRenderbuffer);
		loader.LoadFunction(nameof(glRenderbufferStorage), out glRenderbufferStorage);

		m_frameBuffers  = new Dictionary<OpenGLFrameBufferHandle, uint>();
		m_renderBuffers = new Dictionary<OpenGLRenderBufferHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_frameBuffers.Any())
		{
			var data = m_frameBuffers.Values.ToArray();
			m_frameBuffers.Clear();
			glDeleteFramebuffers(data.Length, data);
		}

		if (m_renderBuffers.Any())
		{
			var data = m_renderBuffers.Values.ToArray();
			m_renderBuffers.Clear();
			glDeleteRenderbuffers(data.Length, data);
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLFrameBufferManager()
	{
		ReleaseUnmanagedResources();
	}

	public bool TryCreateFrameBuffer(out OpenGLFrameBufferHandle frameBuffer)
	{
		var temp = new uint[1];
		glGenFramebuffers(1, temp);
		if (temp[0] == 0u)
		{
			frameBuffer = null!;
			return false;
		}
		else
		{
			frameBuffer = new OpenGLFrameBufferHandle();
			m_frameBuffers.Add(frameBuffer, temp[0]);
			return true;
		}
	}

	public void DeleteFrameBuffer(OpenGLFrameBufferHandle frameBuffer)
	{
		if (m_frameBuffers.Remove(frameBuffer, out var data))
		{
			glDeleteFramebuffers(1, new[] { data });
		}
	}

	public bool TryCreateRenderBuffer(out OpenGLRenderBufferHandle renderBuffer)
	{
		var temp = new uint[1];
		glGenRenderbuffers(1, temp);
		if (temp[0] == 0u)
		{
			renderBuffer = null!;
			return false;
		}
		else
		{
			renderBuffer = new OpenGLRenderBufferHandle();
			m_renderBuffers.Add(renderBuffer, temp[0]);
			return true;
		}
	}

	public void DeleteRenderBuffer(OpenGLRenderBufferHandle renderBuffer)
	{
		if (m_renderBuffers.Remove(renderBuffer, out var data))
		{
			glDeleteRenderbuffers(1, new[] { data });
		}
	}
}
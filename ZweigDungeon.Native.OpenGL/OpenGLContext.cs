﻿using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Prototypes;
using ZweigDungeon.Native.OpenGL.Resources;

namespace ZweigDungeon.Native.OpenGL;

public sealed class OpenGLContext : IDisposable, IVideoContext, IVideoDeviceListener
{
	private readonly IPlatformVideo m_video;
	private readonly IDisposable    m_deviceSubscription;

	// ReSharper disable InconsistentNaming
	private PfnEnableDelegate        glEnable     = null!;
	private PfnDisableDelegate       glDisable    = null!;
	private PfnClearColorDelegate    glClearColor = null!;
	private PfnClearDepthDelegate    glClearDepth = null!;
	private PfnClearDelegate         glClear      = null!;
	private PfnBlendColorDelegate    glBlendColor = null!;
	private PfnBlendFunctionDelegate glBlendFunc  = null!;
	private PfnDepthFunctionDelegate glDepthFunc  = null!;
	private PfnCullFaceDelegate      glCullFace   = null!;
	private PfnFrontFaceDelegate     glFrontFace  = null!;
	private PfnGetErrorDelegate      glGetError   = null!;
	private PfnDepthMaskDelegate     glDepthMask  = null!;
	private PfnColorMaskDelegate     glColorMask  = null!;
	private PfnViewportDelegate      glViewport   = null!;
	private PfnScissorDelegate       glScissor    = null!;
	// ReSharper restore InconsistentNaming

	private OpenGLArrayManager?       m_arrayManager;
	private OpenGLFrameBufferManager? m_frameBufferManager;
	private OpenGLTextureManager?     m_textureManager;
	private OpenGLShaderManager?      m_shaderManager;
	private OpenGLSpriteShader?       m_spriteShader;
	private OpenGLSpriteModel?        m_spriteModel;
	
	public OpenGLContext(IPlatformVideo video, MessageBus messageBus)
	{
		m_video              = video;
		m_deviceSubscription = messageBus.Subscribe<IVideoDeviceListener>(this);
	}

	private void ReleaseUnmanagedResources()
	{
		m_spriteModel?.Dispose();
		m_spriteShader?.Dispose();
		m_arrayManager?.Dispose();
		m_frameBufferManager?.Dispose();
		m_textureManager?.Dispose();
		m_shaderManager?.Dispose();
		m_deviceSubscription.Dispose();
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
		loader.LoadFunction(nameof(glEnable), out glEnable);
		loader.LoadFunction(nameof(glDisable), out glDisable);
		loader.LoadFunction(nameof(glClearColor), out glClearColor);
		loader.LoadFunction(nameof(glClearDepth), out glClearDepth);
		loader.LoadFunction(nameof(glClear), out glClear);
		loader.LoadFunction(nameof(glBlendColor), out glBlendColor);
		loader.LoadFunction(nameof(glBlendFunc), out glBlendFunc);
		loader.LoadFunction(nameof(glDepthMask), out glDepthMask);
		loader.LoadFunction(nameof(glDepthFunc), out glDepthFunc);
		loader.LoadFunction(nameof(glCullFace), out glCullFace);
		loader.LoadFunction(nameof(glFrontFace), out glFrontFace);
		loader.LoadFunction(nameof(glGetError), out glGetError);
		loader.LoadFunction(nameof(glColorMask), out glColorMask);
		loader.LoadFunction(nameof(glViewport), out glViewport);
		loader.LoadFunction(nameof(glScissor), out glScissor);

		m_arrayManager       = new OpenGLArrayManager(loader);
		m_frameBufferManager = new OpenGLFrameBufferManager(loader);
		m_textureManager     = new OpenGLTextureManager(loader);
		m_shaderManager      = new OpenGLShaderManager(loader);
		m_spriteShader       = new OpenGLSpriteShader(m_shaderManager);
		m_spriteModel        = new OpenGLSpriteModel(m_arrayManager);
	}

	public void VideoDeviceDeactivating(IPlatformVideo video)
	{
		if (m_video != video)
		{
			return;
		}

		glEnable     = null!;
		glDisable    = null!;
		glClearColor = null!;
		glClearDepth = null!;
		glClear      = null!;
		glBlendColor = null!;
		glBlendFunc  = null!;
		glDepthFunc  = null!;
		glCullFace   = null!;
		glFrontFace  = null!;
		glGetError   = null!;
		glDepthMask  = null!;
		glColorMask  = null!;
		glViewport   = null!;
		glScissor    = null!;

		m_spriteModel?.Dispose();
		m_spriteShader?.Dispose();
		m_arrayManager?.Dispose();
		m_frameBufferManager?.Dispose();
		m_textureManager?.Dispose();
		m_shaderManager?.Dispose();

		m_spriteModel        = null;
		m_spriteShader       = null;
		m_arrayManager       = null;
		m_frameBufferManager = null;
		m_textureManager     = null;
		m_shaderManager      = null;
	}

	public void BeginFrame(int viewportWidth, int viewportHeight)
	{
		glViewport(0, 0, viewportWidth, viewportHeight);
		glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
		glClearDepth(1.0);
		glClear(OpenGLClearMask.ColorBufferBit | OpenGLClearMask.DepthBufferBit);
		glEnable(OpenGLEnableCap.Blend);
		glBlendFunc(OpenGLBlendSourceFactor.SrcAlpha, OpenGLBlendDestinationFactor.OneMinusSrcAlpha);

		m_spriteShader!.Begin(viewportWidth, viewportHeight);
		m_spriteModel!.Begin();
	}

	public void FinishFrame()
	{
		m_spriteModel?.Finish();
		m_spriteShader?.Finish();
	}
}
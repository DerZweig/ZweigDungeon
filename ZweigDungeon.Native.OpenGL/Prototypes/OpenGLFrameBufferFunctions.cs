using ZweigDungeon.Native.OpenGL.Constants;

namespace ZweigDungeon.Native.OpenGL.Prototypes;

internal delegate void PfnGenFramebuffersDelegate(int count, uint[] framebuffers);

internal delegate void PfnDeleteFramebuffersDelegate(int count, uint[] framebuffers);

internal delegate void PfnBindRenderbufferDelegate(OpenGLRenderBufferTarget target, uint renderbuffer);

internal delegate OpenGLFrameBufferStatus PfnCheckFramebufferStatusDelegate(OpenGLFrameBufferTarget target);

internal delegate void PfnFramebufferRenderbufferDelegate(OpenGLFrameBufferTarget target, OpenGLFrameBufferAttachment attachment, OpenGLRenderBufferTarget renderbufferTarget, uint renderbuffer);

internal delegate void PfnFramebufferTexture2DDelegate(OpenGLFrameBufferTarget target, OpenGLFrameBufferAttachment attachment, OpenGLTextureTarget textureTarget, uint texture, uint level);

internal delegate void PfnGenRenderbuffersDelegate(int count, uint[] renderbuffers);

internal delegate void PfnDeleteRenderbuffersDelegate(int count, uint[] renderbuffers);

internal delegate void PfnRenderbufferStorageDelegate(OpenGLRenderBufferTarget target, OpenGLRenderBufferFormat format, int width, int height);

internal delegate void PfnBindFramebufferDelegate(OpenGLFrameBufferTarget target, uint framebuffer);
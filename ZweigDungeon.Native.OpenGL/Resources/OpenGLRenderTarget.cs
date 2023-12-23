using ZweigDungeon.Common.Services.Video;
using ZweigDungeon.Common.Services.Video.Resources;
using ZweigDungeon.Native.OpenGL.Handles;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLRenderTarget : VideoRenderTarget
{
	public OpenGLRenderTarget(VideoContext context) : base(context)
	{
	}

	public OpenGLFrameBufferHandle FrameBufferHandle { get; set; }
	public OpenGLTexture2D         ColorAttachment   { get; set; }
	public OpenGLRenderTarget      DepthAttachment   { get; set; }
}
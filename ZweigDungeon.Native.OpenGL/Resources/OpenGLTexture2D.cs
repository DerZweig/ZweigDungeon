using ZweigDungeon.Common.Services.Video;
using ZweigDungeon.Common.Services.Video.Resources;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Handles;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLTexture2D : VideoTexture2D
{
	public OpenGLTexture2D(VideoContext context) : base(context)
	{
	}

	public OpenGLTexture2DHandle        Texture2DHandle { get; set; } = null!;
	public int                          Width           { get; set; }
	public int                          Height          { get; set; }
	public OpenGLTextureComponentFormat ComponentFormat { get; set; }
	public OpenGLTextureInternalFormat  InternalFormat  { get; set; }
	public bool                         EnableFiltering { get; set; }
	public bool                         EnableRepeat    { get; set; }
}
using ZweigDungeon.Common.Services.Video;
using ZweigDungeon.Common.Services.Video.Resources;
using ZweigDungeon.Native.OpenGL.Handles;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLShader : VideoShader
{
	public OpenGLShader(VideoContext context) : base(context)
	{
	}

	public OpenGLFragmentShaderHandle FragmentShaderHandle { get; set; } = null!;
	public OpenGLVertexShaderHandle   VertexShaderHandle   { get; set; } = null!;
	public OpenGLProgramHandle        ProgramHandle        { get; set; } = null!;
}
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Handles;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal class OpenGLVertexAttributeDesc
{
	public bool                     IsInstanceParameter { get; init; }
	public OpenGLVertexBufferHandle VertexBuffer        { get; init; } = null!;
	public int                      ElementCount        { get; init; }
	public OpenGLVertexDataType     ElementType         { get; init; }
	public bool                     Normalize           { get; init; }
	public int                      Stride              { get; init; }
	public IntPtr                   Offset              { get; init; }
}
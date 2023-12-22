namespace ZweigDungeon.Native.OpenGL.Constants;

[Flags]
internal enum OpenGLClearMask
{
	DepthBufferBit   = 0x00000100,
	ColorBufferBit   = 0x00004000,
	StencilBufferBit = 0x00000400
}
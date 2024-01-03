namespace ZweigEngine.Native.OpenGL.Constants;

internal enum OpenGLFrameBufferStatus
{
	Complete                    = 0x8CD5,
	IncompleteAttachment        = 0x8CD6,
	IncompleteMissingAttachment = 0x8CD7,
	IncompleteDrawBuffer        = 0x8CDB,
	IncompleteReadBuffer        = 0x8CDC,
	Unsupported                 = 0x8CDD
}
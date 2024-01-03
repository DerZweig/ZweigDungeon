using ZweigEngine.Native.OpenGL.Constants;

namespace ZweigEngine.Native.OpenGL.Prototypes;

internal delegate void PfnBindBufferDelegate(OpenGLBufferTarget target, uint buffer);

internal delegate void PfnBufferDataDelegate(OpenGLBufferTarget target, ulong size, IntPtr data, OpenGLBufferUsage usage);

internal delegate void PfnBufferSubDataDelegate(OpenGLBufferTarget target, ulong offset, ulong size, IntPtr data);

internal delegate void PfnDeleteBuffersDelegate(int count, uint[] buffers);

internal delegate void PfnGenBuffersDelegate(int count, uint[] buffers);
using ZweigDungeon.Native.OpenGL.Constants;

namespace ZweigDungeon.Native.OpenGL.Prototypes;

internal delegate void PfnBindVertexArrayDelegate(uint array);

internal delegate void PfnDeleteVertexArraysDelegate(int count, uint[] arrays);

internal delegate void PfnDisableVertexAttributeArrayDelegate(uint index);

internal delegate void PfnDrawArraysDelegate(OpenGLVertexMode mode, int firstVertexIndex, uint vertexCount);

internal delegate void PfnDrawArraysInstancedDelegate(OpenGLVertexMode mode, int firstVertex, uint vertexCount, uint instanceCount);

internal delegate void PfnEnableVertexAttributeArrayDelegate(uint index);

internal delegate void PfnGenVertexArraysDelegate(int count, uint[] arrays);

internal delegate void PfnVertexAttributeDivisorDelegate(uint index, uint divisor);

internal delegate void PfnVertexAttributePointerDelegate(uint index, int size, OpenGLVertexDataType type, bool normalized, int stride, UIntPtr offset);
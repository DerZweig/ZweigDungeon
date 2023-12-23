using ZweigDungeon.Native.OpenGL.Constants;

namespace ZweigDungeon.Native.OpenGL.Prototypes;

internal delegate void PfnGenQueriesDelegate(int count, uint[] queries);

internal delegate void PfnDeleteQueriesDelegate(int count, uint[] queries);

internal delegate void PfnQueryCounterDelegate(uint query, OpenGLQueryTarget target);

internal delegate void PfnGetQueryObjectIVDelegate(uint query, OpenGLQueryParameter parameter, ref int value);

internal delegate void PfnGetQueryObjectUI64VDelegate(uint query,  OpenGLQueryParameter parameter, ref ulong value);

using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Backend;

internal class OpenGLQueryBackend : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnGenQueriesDelegate          glGenQueries;
	private readonly PfnDeleteQueriesDelegate       glDeleteQueries;
	private readonly PfnQueryCounterDelegate        glQueryCounter;
	private readonly PfnGetQueryObjectIVDelegate    glGetQueryObjectiv;
	private readonly PfnGetQueryObjectUI64VDelegate glGetQueryObjectui64v;
	// ReSharper restore InconsistentNaming

	public OpenGLQueryBackend(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glGenQueries), out glGenQueries);
		loader.LoadFunction(nameof(glDeleteQueries), out glDeleteQueries);
		loader.LoadFunction(nameof(glQueryCounter), out glQueryCounter);
		loader.LoadFunction(nameof(glGetQueryObjectiv), out glGetQueryObjectiv);
		loader.LoadFunction(nameof(glGetQueryObjectui64v), out glGetQueryObjectui64v);
	}

	private void ReleaseUnmanagedResources()
	{
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLQueryBackend()
	{
		ReleaseUnmanagedResources();
	}
}
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Handles;
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

	private readonly Dictionary<OpenGLQueryHandle, uint> m_queries;
	
	public OpenGLQueryBackend(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glGenQueries), out glGenQueries);
		loader.LoadFunction(nameof(glDeleteQueries), out glDeleteQueries);
		loader.LoadFunction(nameof(glQueryCounter), out glQueryCounter);
		loader.LoadFunction(nameof(glGetQueryObjectiv), out glGetQueryObjectiv);
		loader.LoadFunction(nameof(glGetQueryObjectui64v), out glGetQueryObjectui64v);

		m_queries = new Dictionary<OpenGLQueryHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		var queryHandles = m_queries.Values.ToArray();
		m_queries.Clear();

		if (queryHandles.Any())
		{
			glDeleteQueries(queryHandles.Length, queryHandles);
		}
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
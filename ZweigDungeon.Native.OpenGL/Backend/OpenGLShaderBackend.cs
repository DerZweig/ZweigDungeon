using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Backend;

internal sealed class OpenGLShaderBackend : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnCreateProgramDelegate        glCreateProgram;
	private readonly PfnDeleteProgramDelegate        glDeleteProgram;
	private readonly PfnUseProgramDelegate           glUseProgram;
	private readonly PfnLinkProgramDelegate          glLinkProgram;
	private readonly PfnGetProgramDelegate           glGetProgramiv;
	private readonly PfnGetProgramInfoLogDelegate    glGetProgramInfoLog;
	private readonly PfnAttachShaderDelegate         glAttachShader;
	private readonly PfnDetachShaderDelegate         glDetachShader;
	private readonly PfnCreateShaderDelegate         glCreateShader;
	private readonly PfnDeleteShaderDelegate         glDeleteShader;
	private readonly PfnShaderSourceDelegate         glShaderSource;
	private readonly PfnCompileShaderDelegate        glCompileShader;
	private readonly PfnGetShaderDelegate            glGetShaderiv;
	private readonly PfnGetShaderInfoLogDelegate     glGetShaderInfoLog;
	private readonly PfnGetUniformLocationDelegate   glGetUniformLocation;
	private readonly PfnGetAttributeLocationDelegate glGetAttribLocation;
	private readonly PfnUniform1IDelegate            glUniform1i;
	private readonly PfnUniform1FDelegate            glUniform1f;
	private readonly PfnUniform2FDelegate            glUniform2f;
	private readonly PfnUniform3FDelegate            glUniform3f;
	private readonly PfnUniform4FDelegate            glUniform4f;
	private readonly PfnUniformMatrix4FvDelegate     glUniformMatrix4fv;
	// ReSharper restore InconsistentNaming

	private readonly Dictionary<OpenGLFragmentShaderHandle, uint> m_fragmentShaders;
	private readonly Dictionary<OpenGLVertexShaderHandle, uint>   m_vertexShaders;
	private readonly Dictionary<OpenGLProgramHandle, uint>        m_programs;

	public OpenGLShaderBackend(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glCreateProgram), out glCreateProgram);
		loader.LoadFunction(nameof(glDeleteProgram), out glDeleteProgram);
		loader.LoadFunction(nameof(glUseProgram), out glUseProgram);
		loader.LoadFunction(nameof(glLinkProgram), out glLinkProgram);
		loader.LoadFunction(nameof(glGetProgramiv), out glGetProgramiv);
		loader.LoadFunction(nameof(glGetProgramInfoLog), out glGetProgramInfoLog);
		loader.LoadFunction(nameof(glAttachShader), out glAttachShader);
		loader.LoadFunction(nameof(glDetachShader), out glDetachShader);
		loader.LoadFunction(nameof(glCreateShader), out glCreateShader);
		loader.LoadFunction(nameof(glDeleteShader), out glDeleteShader);
		loader.LoadFunction(nameof(glShaderSource), out glShaderSource);
		loader.LoadFunction(nameof(glCompileShader), out glCompileShader);
		loader.LoadFunction(nameof(glGetShaderiv), out glGetShaderiv);
		loader.LoadFunction(nameof(glGetShaderInfoLog), out glGetShaderInfoLog);
		loader.LoadFunction(nameof(glGetUniformLocation), out glGetUniformLocation);
		loader.LoadFunction(nameof(glGetAttribLocation), out glGetAttribLocation);
		loader.LoadFunction(nameof(glUniform1i), out glUniform1i);
		loader.LoadFunction(nameof(glUniform1f), out glUniform1f);
		loader.LoadFunction(nameof(glUniform2f), out glUniform2f);
		loader.LoadFunction(nameof(glUniform3f), out glUniform3f);
		loader.LoadFunction(nameof(glUniform4f), out glUniform4f);
		loader.LoadFunction(nameof(glUniformMatrix4fv), out glUniformMatrix4fv);

		m_fragmentShaders = new Dictionary<OpenGLFragmentShaderHandle, uint>();
		m_vertexShaders   = new Dictionary<OpenGLVertexShaderHandle, uint>();
		m_programs        = new Dictionary<OpenGLProgramHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		var programHandles        = m_programs.Values.ToArray();
		var fragmentShaderHandles = m_fragmentShaders.Values.ToArray();
		var vertexShaderHandles   = m_vertexShaders.Values.ToArray();

		m_programs.Clear();
		m_fragmentShaders.Clear();
		m_vertexShaders.Clear();

		foreach (var programHandle in programHandles)
		{
			glDeleteProgram(programHandle);
		}

		foreach (var fragmentShaderHandle in fragmentShaderHandles)
		{
			glDeleteShader(fragmentShaderHandle);
		}

		foreach (var vertexShaderHandle in vertexShaderHandles)
		{
			glDeleteShader(vertexShaderHandle);
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLShaderBackend()
	{
		ReleaseUnmanagedResources();
	}
}
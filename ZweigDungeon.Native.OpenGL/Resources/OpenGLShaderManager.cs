using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal class OpenGLShaderManager : IDisposable
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

	private readonly Dictionary<OpenGLProgramHandle, uint>        m_programs;
	private readonly Dictionary<OpenGLFragmentShaderHandle, uint> m_fragmentShaders;
	private readonly Dictionary<OpenGLVertexShaderHandle, uint>   m_vertexShaders;

	private OpenGLProgramHandle? m_currentProgram;

	public OpenGLShaderManager(ICustomFunctionLoader loader)
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

		m_programs        = new Dictionary<OpenGLProgramHandle, uint>();
		m_fragmentShaders = new Dictionary<OpenGLFragmentShaderHandle, uint>();
		m_vertexShaders   = new Dictionary<OpenGLVertexShaderHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_programs.Any())
		{
			var data = m_programs.Values.ToArray();
			m_programs.Clear();

			foreach (var entry in data)
			{
				glDeleteProgram(entry);
			}
		}

		if (m_fragmentShaders.Any())
		{
			var data = m_fragmentShaders.Values.ToArray();
			m_fragmentShaders.Clear();
			foreach (var entry in data)
			{
				glDeleteShader(entry);
			}
		}

		if (m_vertexShaders.Any())
		{
			var data = m_vertexShaders.Values.ToArray();
			m_vertexShaders.Clear();
			foreach (var entry in data)
			{
				glDeleteShader(entry);
			}
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLShaderManager()
	{
		ReleaseUnmanagedResources();
	}

	public bool TryCreateProgram(out OpenGLProgramHandle program)
	{
		var value = glCreateProgram();
		if (value != 0u)
		{
			program = new OpenGLProgramHandle();
			m_programs.Add(program, value);
			return true;
		}
		else
		{
			program = null!;
			return false;
		}
	}

	public void DeleteProgram(OpenGLProgramHandle program)
	{
		if (m_programs.Remove(program, out var data))
		{
			glDeleteProgram(data);
		}
	}

	public void BindProgram(OpenGLProgramHandle program)
	{
		if (m_programs.TryGetValue(program, out var prog))
		{
			if (m_currentProgram != program)
			{
				glUseProgram(prog);
				m_currentProgram = program;
			}
		}
		else
		{
			UnbindProgram();
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(BindProgram)} attempting to bind uninitialized program.");
		}
	}

	public void UnbindProgram()
	{
		if (m_currentProgram != null)
		{
			glUseProgram(0u);
			m_currentProgram = null;
		}
	}

	public bool LinkProgram(OpenGLProgramHandle program, OpenGLVertexShaderHandle vertexShader, OpenGLFragmentShaderHandle fragmentShader)
	{
		if (!m_programs.TryGetValue(program, out var prog))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(LinkProgram)} attempting to bind uninitialized program.");
		}

		if (!m_fragmentShaders.TryGetValue(fragmentShader, out var frag))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(LinkProgram)} attempting to access uninitialized fragment shader.");
		}

		if (!m_vertexShaders.TryGetValue(vertexShader, out var vert))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(LinkProgram)} attempting to access uninitialized vertex shader.");
		}

		glAttachShader(prog, frag);
		glAttachShader(prog, vert);
		glLinkProgram(prog);

		var linkStatus = 0;
		glGetProgramiv(prog, OpenGLProgramProperty.LinkStatus, ref linkStatus);
		return linkStatus != 0;
	}

	public int GetProgramInputBinding(OpenGLProgramHandle program, string uniformName)
	{
		if (!m_programs.TryGetValue(program, out var prog))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(GetProgramInputBinding)} attempting to bind uninitialized program.");
		}

		return glGetUniformLocation(prog, uniformName);
	}
	
	public void SetProgramInput(OpenGLProgramHandle program, int bindingLocation, int v1)
	{
		BindProgram(program);
		glUniform1i(bindingLocation, v1);
	}

	public void SetProgramInput(OpenGLProgramHandle program, int bindingLocation, float v1)
	{
		BindProgram(program);
		glUniform1f(bindingLocation, v1);
	}
	
	public void SetProgramInput(OpenGLProgramHandle program, int bindingLocation, float v1, float v2)
	{
		BindProgram(program);
		glUniform2f(bindingLocation, v1, v2);
	}
	
	public void SetProgramInput(OpenGLProgramHandle program, int bindingLocation, float v1, float v2, float v3)
	{
		BindProgram(program);
		glUniform3f(bindingLocation, v1, v2, v3);
	}
	
	public void SetProgramInput(OpenGLProgramHandle program, int bindingLocation, float v1, float v2, float v3, float v4)
	{
		BindProgram(program);
		glUniform4f(bindingLocation, v1, v2, v3, v4);
	}

	public bool TryCreateFragmentShader(out OpenGLFragmentShaderHandle fragmentShader)
	{
		var value = glCreateShader(OpenGLShaderType.Fragment);
		if (value != 0u)
		{
			fragmentShader = new OpenGLFragmentShaderHandle();
			m_fragmentShaders.Add(fragmentShader, value);
			return true;
		}
		else
		{
			fragmentShader = null!;
			return false;
		}
	}

	public void DeleteFragmentShader(OpenGLFragmentShaderHandle fragmentShader)
	{
		if (m_fragmentShaders.Remove(fragmentShader, out var data))
		{
			glDeleteShader(data);
		}
	}

	public bool CompileFragmentShader(OpenGLFragmentShaderHandle fragmentShader, Stream sourceStream)
	{
		if (!m_fragmentShaders.TryGetValue(fragmentShader, out var shader))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(CompileFragmentShader)} attempting to access uninitialized fragment shader.");
		}

		using (var reader = new StreamReader(sourceStream))
		{
			var source = reader.ReadToEnd();
			glShaderSource(shader, 1u, new[] { source }, null!);
			glCompileShader(shader);
		}

		var compileStatus = 0;
		glGetShaderiv(shader, OpenGLShaderProperty.CompileStatus, ref compileStatus);
		return compileStatus != 0;
	}

	public bool TryCreateVertexShader(out OpenGLVertexShaderHandle vertexShader)
	{
		var value = glCreateShader(OpenGLShaderType.Vertex);
		if (value != 0u)
		{
			vertexShader = new OpenGLVertexShaderHandle();
			m_vertexShaders.Add(vertexShader, value);
			return true;
		}
		else
		{
			vertexShader = null!;
			return false;
		}
	}

	public void DeleteVertexShader(OpenGLVertexShaderHandle vertexShader)
	{
		if (m_vertexShaders.Remove(vertexShader, out var data))
		{
			glDeleteShader(data);
		}
	}

	public bool CompileVertexShader(OpenGLVertexShaderHandle vertexShader, Stream sourceStream)
	{
		if (!m_vertexShaders.TryGetValue(vertexShader, out var shader))
		{
			throw new AccessViolationException($"{nameof(OpenGLShaderManager)}::{nameof(CompileVertexShader)} attempting to access uninitialized vertex shader.");
		}

		using (var reader = new StreamReader(sourceStream))
		{
			var source = reader.ReadToEnd();
			glShaderSource(shader, 1u, new[] { source }, null!);
			glCompileShader(shader);
		}

		var compileStatus = 0;
		glGetShaderiv(shader, OpenGLShaderProperty.CompileStatus, ref compileStatus);
		return compileStatus != 0;
	}
}
using System.Reflection;
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Renderer;

internal class OpenGLSpriteShader : IDisposable
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

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

	// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
	// ReSharper restore InconsistentNaming

	private uint m_program;
	private uint m_fragmentShader;
	private uint m_vertexShader;

	private int m_viewportUniformLocation;
	private int m_textureUniformLocation;

	public OpenGLSpriteShader(ICustomFunctionLoader loader)
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

		m_program        = glCreateProgram();
		m_fragmentShader = glCreateShader(OpenGLShaderType.Fragment);
		m_vertexShader   = glCreateShader(OpenGLShaderType.Vertex);

		if (m_program == 0u || m_fragmentShader == 0u || m_vertexShader == 0u)
		{
			throw new Exception("Couldn't allocate sprite shader program.");
		}

		var assembly           = Assembly.GetAssembly(typeof(OpenGLSpriteShader)) ?? throw new AccessViolationException("Couldn't load sprite shader resource assembly.");
		var resourceNames      = assembly.GetManifestResourceNames();
		var fragmentShaderName = resourceNames.FirstOrDefault(x => x.EndsWith("sprite.frag.glsl")) ?? throw new AccessViolationException("Couldn't locate sprite pixel shader resource.");
		var vertexShaderName   = resourceNames.FirstOrDefault(x => x.EndsWith("sprite.vert.glsl")) ?? throw new AccessViolationException("Couldn't locate sprite vertex shader resource.");

		using (var fragmentShaderStream = assembly.GetManifestResourceStream(fragmentShaderName))
		using (var fragmentShaderReader = new StreamReader(fragmentShaderStream!))
		{
			var fragmentShaderSource = fragmentShaderReader.ReadToEnd();
			glShaderSource(m_fragmentShader, 1u, new[] { fragmentShaderSource }, null!);
			glCompileShader(m_fragmentShader);
		}

		using (var vertexShaderStream = assembly.GetManifestResourceStream(vertexShaderName))
		using (var vertexShaderRwader = new StreamReader(vertexShaderStream!))
		{
			var vertexShaderSource = vertexShaderRwader.ReadToEnd();
			glShaderSource(m_vertexShader, 1u, new[] { vertexShaderSource }, null!);
			glCompileShader(m_vertexShader);
		}

		var fragmentShaderStatus = 0;
		var vertexShaderStatus   = 0;
		var programStatus        = 0;

		glGetShaderiv(m_fragmentShader, OpenGLShaderProperty.CompileStatus, ref fragmentShaderStatus);
		glGetShaderiv(m_vertexShader, OpenGLShaderProperty.CompileStatus, ref vertexShaderStatus);

		if (fragmentShaderStatus == 0 || vertexShaderStatus == 0)
		{
			throw new Exception("Couldn't compile sprite shader program.");
		}

		glAttachShader(m_program, m_vertexShader);
		glAttachShader(m_program, m_fragmentShader);
		glLinkProgram(m_program);
		glGetProgramiv(m_program, OpenGLProgramProperty.LinkStatus, ref programStatus);
		if (programStatus == 0)
		{
			throw new Exception("Couldn't link sprite shader program");
		}

		m_viewportUniformLocation = glGetUniformLocation(m_program, "uViewport");
		m_textureUniformLocation  = glGetUniformLocation(m_program, "uTextureSprite");
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_program != 0u)
		{
			glDeleteProgram(m_program);
			m_program = 0u;
		}

		if (m_fragmentShader != 0u)
		{
			glDeleteShader(m_fragmentShader);
			m_fragmentShader = 0u;
		}

		if (m_vertexShader != 0u)
		{
			glDeleteShader(m_vertexShader);
			m_vertexShader = 0u;
		}

		m_viewportUniformLocation = 0;
		m_textureUniformLocation  = 0;
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLSpriteShader()
	{
		ReleaseUnmanagedResources();
	}

	public void Begin(int viewportWidth, int viewportHeight)
	{
		glUseProgram(m_program);
		glUniform2f(m_viewportUniformLocation, viewportWidth, viewportHeight);
		glUniform1i(m_textureUniformLocation, 0);
	}

	public void Finish()
	{
		glUseProgram(0u);
	}
}
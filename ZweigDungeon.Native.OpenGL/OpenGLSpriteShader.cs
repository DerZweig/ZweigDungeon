using System.Reflection;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Resources;

namespace ZweigDungeon.Native.OpenGL;

internal class OpenGLSpriteShader : IDisposable
{
	private readonly OpenGLShaderManager        m_shaderManager;
	private readonly OpenGLProgramHandle        m_program;
	private readonly OpenGLFragmentShaderHandle m_fragmentShader;
	private readonly OpenGLVertexShaderHandle   m_vertexShader;

	private int m_viewportUniformLocation;
	private int m_textureUniformLocation;

	public OpenGLSpriteShader(OpenGLShaderManager shaderManager)
	{
		m_shaderManager = shaderManager;

		if (!m_shaderManager.TryCreateProgram(out m_program))
		{
			throw new Exception("Couldn't allocate sprite shader program.");
		}

		if (!m_shaderManager.TryCreateFragmentShader(out m_fragmentShader))
		{
			throw new Exception("Couldn't allocate sprite pixel shader.");
		}

		if (!m_shaderManager.TryCreateVertexShader(out m_vertexShader))
		{
			throw new Exception("Couldn't allocate sprite vertex shader.");
		}

		var assembly           = Assembly.GetAssembly(typeof(OpenGLSpriteShader)) ?? throw new AccessViolationException("Couldn't load sprite shader resource assembly.");
		var resourceNames      = assembly.GetManifestResourceNames();
		var fragmentShaderName = resourceNames.FirstOrDefault(x => x.EndsWith("sprite.frag.glsl")) ?? throw new AccessViolationException("Couldn't locate sprite pixel shader resource.");
		var vertexShaderName   = resourceNames.FirstOrDefault(x => x.EndsWith("sprite.vert.glsl")) ?? throw new AccessViolationException("Couldn't locate sprite vertex shader resource.");

		using (var fragmentShaderStream = assembly.GetManifestResourceStream(fragmentShaderName))
		{
			if (!m_shaderManager.CompileFragmentShader(m_fragmentShader, fragmentShaderStream ?? throw new AccessViolationException("Couldn't open sprite pixel shader resource.")))
			{
				throw new Exception("Couldn't compile sprite pixel shader.");
			}
		}

		using (var vertexShaderStream = assembly.GetManifestResourceStream(vertexShaderName))
		{
			if (!m_shaderManager.CompileVertexShader(m_vertexShader, vertexShaderStream ?? throw new AccessViolationException("Couldn't open sprite vertex shader resource.")))
			{
				throw new Exception("Couldn't compile sprite vertex shader.");
			}
		}

		if (!m_shaderManager.LinkProgram(m_program, m_vertexShader, m_fragmentShader))
		{
			throw new Exception("Couldn't link sprite shader program.");
		}

		m_viewportUniformLocation = m_shaderManager.GetProgramInputBinding(m_program, "uViewport");
		m_textureUniformLocation  = m_shaderManager.GetProgramInputBinding(m_program, "uTextureSprite");
	}

	private void ReleaseUnmanagedResources()
	{
		m_shaderManager.DeleteProgram(m_program);
		m_shaderManager.DeleteFragmentShader(m_fragmentShader);
		m_shaderManager.DeleteVertexShader(m_vertexShader);
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
		m_shaderManager.BindProgram(m_program);
		m_shaderManager.SetProgramInput(m_program, m_viewportUniformLocation, viewportWidth, viewportHeight);
		m_shaderManager.SetProgramInput(m_program, m_textureUniformLocation, 0);
		
	}

	public void Finish()
	{
		m_shaderManager.UnbindProgram();
	}
}
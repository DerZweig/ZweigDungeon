using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal class OpenGLTextureManager : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnActiveTextureDelegate  glActiveTexture;
	private readonly PfnBindTextureDelegate    glBindTexture;
	private readonly PfnDeleteTexturesDelegate glDeleteTextures;
	private readonly PfnGenTexturesDelegate    glGenTextures;
	private readonly PfnTexImage2DDelegate     glTexImage2D;
	private readonly PfnTexParameteriDelegate  glTexParameteri;
	private readonly PfnTexSubImage2DDelegate  glTexSubImage2D;
	// ReSharper restore InconsistentNaming

	private readonly Dictionary<OpenGLTexture2DHandle, uint> m_textures;

	public OpenGLTextureManager(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glActiveTexture), out glActiveTexture);
		loader.LoadFunction(nameof(glBindTexture), out glBindTexture);
		loader.LoadFunction(nameof(glDeleteTextures), out glDeleteTextures);
		loader.LoadFunction(nameof(glGenTextures), out glGenTextures);
		loader.LoadFunction(nameof(glTexImage2D), out glTexImage2D);
		loader.LoadFunction(nameof(glTexParameteri), out glTexParameteri);
		loader.LoadFunction(nameof(glTexSubImage2D), out glTexSubImage2D);

		m_textures = new Dictionary<OpenGLTexture2DHandle, uint>();
	}
	
	private void ReleaseUnmanagedResources()
	{
		if (m_textures.Any())
		{
			var data = m_textures.Values.ToArray();
			m_textures.Clear();
			glDeleteTextures(data.Length, data);
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLTextureManager()
	{
		ReleaseUnmanagedResources();
	}

	public bool TryCreateTexture2D(out OpenGLTexture2DHandle texture)
	{
		var temp = new uint[1];
		glGenTextures(1, temp);
		if (temp[0] == 0u)
		{
			texture = null!;
			return false;
		}
		else
		{
			texture = new OpenGLTexture2DHandle();
			m_textures.Add(texture, temp[0]);
			return true;
		}
	}

	public void DeleteTexture(OpenGLTexture2DHandle texture)
	{
		if (m_textures.Remove(texture, out var data))
		{
			glDeleteTextures(1, new[] { data });
		}
	}
}
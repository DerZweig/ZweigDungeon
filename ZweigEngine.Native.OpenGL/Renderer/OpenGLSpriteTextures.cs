using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Native.OpenGL.Constants;
using ZweigEngine.Native.OpenGL.Prototypes;

namespace ZweigEngine.Native.OpenGL.Renderer;

internal class OpenGLSpriteTextures : IDisposable
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

	private readonly PfnActiveTextureDelegate  glActiveTexture;
	private readonly PfnBindTextureDelegate    glBindTexture;
	private readonly PfnDeleteTexturesDelegate glDeleteTextures;
	private readonly PfnGenTexturesDelegate    glGenTextures;
	private readonly PfnTexImage2DDelegate     glTexImage2D;
	private readonly PfnTexParameteriDelegate  glTexParameteri;
	private readonly PfnTexSubImage2DDelegate  glTexSubImage2D;

	// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
	// ReSharper restore InconsistentNaming

	private readonly List<uint>  m_allocated;
	private readonly Stack<uint> m_freelist;

	public OpenGLSpriteTextures(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glActiveTexture), out glActiveTexture);
		loader.LoadFunction(nameof(glBindTexture), out glBindTexture);
		loader.LoadFunction(nameof(glDeleteTextures), out glDeleteTextures);
		loader.LoadFunction(nameof(glGenTextures), out glGenTextures);
		loader.LoadFunction(nameof(glTexImage2D), out glTexImage2D);
		loader.LoadFunction(nameof(glTexParameteri), out glTexParameteri);
		loader.LoadFunction(nameof(glTexSubImage2D), out glTexSubImage2D);

		m_allocated = new List<uint>();
		m_freelist  = new Stack<uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_allocated.Any())
		{
			glDeleteTextures(m_allocated.Count, m_allocated.ToArray());
		}

		m_allocated.Clear();
		m_freelist.Clear();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLSpriteTextures()
	{
		ReleaseUnmanagedResources();
	}

	public void Bind(uint? name)
	{
		glActiveTexture(OpenGLTextureUnit.Texture0);
		glBindTexture(OpenGLTextureTarget.Texture2D, name ?? 0u);
	}

	public void Release(uint name)
	{
		m_freelist.Push(name);
	}

	public uint Create(ushort width, ushort height, IntPtr data)
	{
		if (!m_freelist.TryPop(out var name))
		{
			var temp = new uint[1];
			glGenTextures(1, temp);
			name = temp[0];

			if (name == 0u)
			{
				throw new OutOfMemoryException("Couldn't allocate additional texture resources.");
			}

			m_allocated.Add(name);
		}

		Upload(name, width, height, data);
		return name;
	}

	public void Upload(uint name, ushort width, ushort height, IntPtr data)
	{
		glActiveTexture(OpenGLTextureUnit.Texture0);
		glBindTexture(OpenGLTextureTarget.Texture2D, name);
		glTexImage2D(OpenGLTextureUploadTarget.Texture2D, 0, OpenGLTextureInternalFormat.RGBA8, width, height, 0, OpenGLTextureComponentFormat.RGBA, OpenGLTextureDataType.UnsignedByte, data);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureMinFilter, OpenGLTextureParameter.Nearest);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureMagFilter, OpenGLTextureParameter.Nearest);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureWrapS, OpenGLTextureParameter.Clamp);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureWrapT, OpenGLTextureParameter.Clamp);
	}
}
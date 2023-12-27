using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Renderer;

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

	private readonly List<uint>                      m_allocated;
	private readonly Stack<uint>                     m_freelist;
	private readonly Dictionary<OpenGLSurface, uint> m_mappings;

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
		m_mappings  = new Dictionary<OpenGLSurface, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_mappings.Any())
		{
			glDeleteTextures(m_allocated.Count, m_allocated.ToArray());
		}
		
		m_allocated.Clear();
		m_freelist.Clear();
		m_mappings.Clear();
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
	
	public OpenGLSurface? ActiveSurface { get; private set; }

	public void Bind(OpenGLSurface? surface)
	{
		if (surface == null)
		{
			glActiveTexture(OpenGLTextureUnit.Texture0);
			glBindTexture(OpenGLTextureTarget.Texture2D, 0u);
			ActiveSurface = null;
		}
		else if (!m_mappings.TryGetValue(surface, out var texture))
		{
			Upload(surface);
			ActiveSurface = surface;
		}
		else
		{
			glActiveTexture(OpenGLTextureUnit.Texture0);
			glBindTexture(OpenGLTextureTarget.Texture2D, texture);
			ActiveSurface = surface;
		}
	}

	public void Release(OpenGLSurface surface)
	{
		if (m_mappings.Remove(surface, out var texture))
		{
			m_freelist.Push(texture);
		}
	}

	public void Upload(OpenGLSurface surface)
	{
		if (!m_mappings.TryGetValue(surface, out var texture))
		{
			if (!m_freelist.TryPop(out texture))
			{
				var temp = new uint[1];
				glGenTextures(1, temp);
				texture = temp[0];
				
				if (texture == 0u)
				{
					throw new OutOfMemoryException("Couldn't allocate additional texture resources.");
				}
				
				m_allocated.Add(texture);
			}
			
			m_mappings[surface] = texture;
		}
		
		ActiveSurface = surface;
		glActiveTexture(OpenGLTextureUnit.Texture0);
		glBindTexture(OpenGLTextureTarget.Texture2D, texture);
		UploadInternal(surface.Width, surface.Height, surface.Address);
	}

	private void UploadInternal(ushort width, ushort height, IntPtr data)
	{
		glTexImage2D(OpenGLTextureUploadTarget.Texture2D, 0, OpenGLTextureInternalFormat.RGBA8, width, height, 0, OpenGLTextureComponentFormat.RGBA, OpenGLTextureDataType.UnsignedByte, data);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureMinFilter, OpenGLTextureParameter.Nearest);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureMagFilter, OpenGLTextureParameter.Nearest);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureWrapS, OpenGLTextureParameter.Clamp);
		glTexParameteri(OpenGLTextureTarget.Texture2D, OpenGLTextureParameterName.TextureWrapT, OpenGLTextureParameter.Clamp);
	}
}
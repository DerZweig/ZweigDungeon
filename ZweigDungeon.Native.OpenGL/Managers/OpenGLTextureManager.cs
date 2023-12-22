using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Managers;

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

	public OpenGLTextureManager(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glActiveTexture), out glActiveTexture);
		loader.LoadFunction(nameof(glBindTexture), out glBindTexture);
		loader.LoadFunction(nameof(glDeleteTextures), out glDeleteTextures);
		loader.LoadFunction(nameof(glGenTextures), out glGenTextures);
		loader.LoadFunction(nameof(glTexImage2D), out glTexImage2D);
		loader.LoadFunction(nameof(glTexParameteri), out glTexParameteri);
		loader.LoadFunction(nameof(glTexSubImage2D), out glTexSubImage2D);
	}

	private void ReleaseUnmanagedResources()
	{
		// TODO release unmanaged resources here
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
}
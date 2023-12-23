using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Backend;

internal sealed class OpenGLArrayBackend : IDisposable
{
	// ReSharper disable InconsistentNaming
	private readonly PfnGenVertexArraysDelegate             glGenVertexArrays;
	private readonly PfnDeleteVertexArraysDelegate          glDeleteVertexArrays;
	private readonly PfnBindVertexArrayDelegate             glBindVertexArray;
	private readonly PfnEnableVertexAttributeArrayDelegate  glEnableVertexAttribArray;
	private readonly PfnDisableVertexAttributeArrayDelegate glDisableVertexAttribArray;
	private readonly PfnVertexAttributePointerDelegate      glVertexAttribPointer;
	private readonly PfnVertexAttributeDivisorDelegate      glVertexAttribDivisor;
	private readonly PfnDrawArraysDelegate                  glDrawArrays;
	private readonly PfnDrawArraysInstancedDelegate         glDrawArraysInstanced;
	private readonly PfnGenBuffersDelegate                  glGenBuffers;
	private readonly PfnDeleteBuffersDelegate               glDeleteBuffers;
	private readonly PfnBindBufferDelegate                  glBindBuffer;
	private readonly PfnBufferDataDelegate                  glBufferData;
	private readonly PfnBufferSubDataDelegate               glBufferSubData;
	// ReSharper restore InconsistentNaming
	
	public OpenGLArrayBackend(ICustomFunctionLoader loader)
	{
		loader.LoadFunction(nameof(glGenVertexArrays), out glGenVertexArrays);
		loader.LoadFunction(nameof(glDeleteVertexArrays), out glDeleteVertexArrays);
		loader.LoadFunction(nameof(glBindVertexArray), out glBindVertexArray);
		loader.LoadFunction(nameof(glEnableVertexAttribArray), out glEnableVertexAttribArray);
		loader.LoadFunction(nameof(glDisableVertexAttribArray), out glDisableVertexAttribArray);
		loader.LoadFunction(nameof(glVertexAttribPointer), out glVertexAttribPointer);
		loader.LoadFunction(nameof(glVertexAttribDivisor), out glVertexAttribDivisor);
		loader.LoadFunction(nameof(glDrawArrays), out glDrawArrays);
		loader.LoadFunction(nameof(glDrawArraysInstanced), out glDrawArraysInstanced);
		loader.LoadFunction(nameof(glGenBuffers), out glGenBuffers);
		loader.LoadFunction(nameof(glDeleteBuffers), out glDeleteBuffers);
		loader.LoadFunction(nameof(glBindBuffer), out glBindBuffer);
		loader.LoadFunction(nameof(glBufferData), out glBufferData);
		loader.LoadFunction(nameof(glBufferSubData), out glBufferSubData);
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

	~OpenGLArrayBackend()
	{
		ReleaseUnmanagedResources();
	}

}
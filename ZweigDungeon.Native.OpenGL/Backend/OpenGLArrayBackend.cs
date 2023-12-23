using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Handles;
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

	private readonly Dictionary<OpenGLArrayHandle, uint>  m_arrays;
	private readonly Dictionary<OpenGLBufferHandle, uint> m_buffers;

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

		m_arrays  = new Dictionary<OpenGLArrayHandle, uint>();
		m_buffers = new Dictionary<OpenGLBufferHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		var arrayHandles  = m_arrays.Values.ToArray();
		var bufferHandles = m_buffers.Values.ToArray();
		
		m_arrays.Clear();
		m_buffers.Clear();

		if (arrayHandles.Any())
		{
			glDeleteVertexArrays(arrayHandles.Length, arrayHandles);
		}

		if (bufferHandles.Any())
		{
			glDeleteBuffers(bufferHandles.Length, bufferHandles);
		}
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
using ZweigDungeon.Common.Interfaces.Libraries;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Prototypes;

namespace ZweigDungeon.Native.OpenGL.Resources;

internal sealed class OpenGLArrayManager : IDisposable
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

	private readonly Dictionary<OpenGLVertexArrayHandle, uint>  m_vertexArrays;
	private readonly Dictionary<OpenGLVertexBufferHandle, uint> m_vertexBuffer;

	private OpenGLVertexArrayHandle?  m_currentArray;
	private OpenGLVertexBufferHandle? m_currentBuffer;

	public OpenGLArrayManager(ICustomFunctionLoader loader)
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

		m_vertexArrays = new Dictionary<OpenGLVertexArrayHandle, uint>();
		m_vertexBuffer = new Dictionary<OpenGLVertexBufferHandle, uint>();
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_vertexArrays.Any())
		{
			var data = m_vertexArrays.Values.ToArray();
			m_vertexArrays.Clear();
			glDeleteVertexArrays(data.Length, data);
		}

		if (m_vertexBuffer.Any())
		{
			var data = m_vertexBuffer.Values.ToArray();
			m_vertexBuffer.Clear();
			glDeleteBuffers(data.Length, data);
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLArrayManager()
	{
		ReleaseUnmanagedResources();
	}

	public bool TryCreateVertexArray(out OpenGLVertexArrayHandle vertexArray)
	{
		var temp = new uint[1];
		glGenVertexArrays(1, temp);
		if (temp[0] == 0u)
		{
			vertexArray = null!;
			return false;
		}
		else
		{
			vertexArray = new OpenGLVertexArrayHandle();
			m_vertexArrays.Add(vertexArray, temp[0]);
			return true;
		}
	}

	public void DeleteVertexArray(OpenGLVertexArrayHandle vertexArray)
	{
		if (m_vertexArrays.Remove(vertexArray, out var data))
		{
			glDeleteVertexArrays(1, new[] { data });
		}
	}

	public void BindArray(OpenGLVertexArrayHandle vertexArray)
	{
		if (m_vertexArrays.TryGetValue(vertexArray, out var vao))
		{
			if (m_currentArray != vertexArray)
			{
				glBindVertexArray(vao);
				m_currentArray = vertexArray;
			}
		}
		else
		{
			UnbindArray();
			throw new AccessViolationException($"{nameof(OpenGLArrayManager)}::{nameof(BindArray)} attempting to bind uninitialized array.");
		}
	}

	public void UnbindArray()
	{
		if (m_currentArray != null)
		{
			glBindVertexArray(0u);
			m_currentArray = null;
		}
	}

	public void InitializeVertexArray(OpenGLVertexArrayHandle vertexArray, IReadOnlyList<OpenGLVertexAttributeDesc> attributeDescriptors)
	{
		BindArray(vertexArray);
		try
		{
			var index = 0u;
			foreach (var desc in attributeDescriptors)
			{
				glEnableVertexAttribArray(index);
				BindBuffer(desc.VertexBuffer);
				glVertexAttribPointer(index, desc.ElementCount, desc.ElementType, desc.Normalize, desc.Stride, (UIntPtr)desc.Offset);
				UnbindBuffer();
				if (desc.IsInstanceParameter)
				{
					glVertexAttribDivisor(index, 1);
				}

				++index;
			}
		}
		finally
		{
			UnbindArray();
		}
	}

	public void DrawArray(OpenGLVertexArrayHandle vertexArray, OpenGLVertexMode vertexMode, int firstVertex, int vertexCount)
	{
		BindArray(vertexArray);
		glDrawArrays(vertexMode, firstVertex, (uint)vertexCount);
	}

	public void DrawArrayInstance(OpenGLVertexArrayHandle vertexArray, OpenGLVertexMode vertexMode, int firstVertex, int vertexCount, int instanceCount)
	{
		BindArray(vertexArray);
		glDrawArraysInstanced(vertexMode, firstVertex, (uint)vertexCount, (uint)instanceCount);
	}

	public bool TryCreateVertexBuffer(out OpenGLVertexBufferHandle vertexBuffer)
	{
		var temp = new uint[1];
		glGenBuffers(1, temp);
		if (temp[0] == 0u)
		{
			vertexBuffer = null!;
			return false;
		}
		else
		{
			vertexBuffer = new OpenGLVertexBufferHandle();
			m_vertexBuffer.Add(vertexBuffer, temp[0]);
			return true;
		}
	}

	public void DeleteVertexBuffer(OpenGLVertexBufferHandle vertexBuffer)
	{
		if (m_vertexBuffer.Remove(vertexBuffer, out var data))
		{
			glDeleteBuffers(1, new[] { data });
		}
	}

	public void BindBuffer(OpenGLVertexBufferHandle vertexBuffer)
	{
		if (m_vertexBuffer.TryGetValue(vertexBuffer, out var vbo))
		{
			if (m_currentBuffer != vertexBuffer)
			{
				glBindBuffer(OpenGLBufferTarget.Array, vbo);
				m_currentBuffer = vertexBuffer;
			}
		}
		else
		{
			UnbindBuffer();
			throw new AccessViolationException($"{nameof(OpenGLArrayManager)}::{nameof(BindBuffer)} attempting to bind uninitialized buffer.");
		}
	}

	public void UnbindBuffer()
	{
		if (m_currentBuffer != null)
		{
			glBindBuffer(OpenGLBufferTarget.Array, 0u);
			m_currentBuffer = null;
		}
	}

	public void InitializeVertexBuffer(OpenGLVertexBufferHandle vertexBuffer, int size, IntPtr data, OpenGLBufferUsage usage)
	{
		BindBuffer(vertexBuffer);
		glBufferData(OpenGLBufferTarget.Array, (ulong)size, data, usage);
		UnbindBuffer();
	}

	public void UpdateVertexBuffer(OpenGLVertexBufferHandle vertexBuffer, int size, IntPtr data, OpenGLBufferUsage usage)
	{
		BindBuffer(vertexBuffer);
		if (usage is OpenGLBufferUsage.StreamCopy or OpenGLBufferUsage.StreamDraw or OpenGLBufferUsage.StreamRead)
		{
			glBufferData(OpenGLBufferTarget.Array, (ulong)size, IntPtr.Zero, usage);
			glBufferData(OpenGLBufferTarget.Array, (ulong)size, data, usage);
		}
		else
		{
			glBufferData(OpenGLBufferTarget.Array, (ulong)size, data, usage);
		}

		UnbindBuffer();
	}
}
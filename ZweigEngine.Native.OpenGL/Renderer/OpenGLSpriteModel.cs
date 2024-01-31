using System.Runtime.InteropServices;
using ZweigEngine.Common.Services.Interfaces.Libraries;
using ZweigEngine.Common.Services.Interfaces.Video;
using ZweigEngine.Common.Services.Interfaces.Video.Constant;
using ZweigEngine.Common.Services.Interfaces.Video.Structures;
using ZweigEngine.Common.Utility.Interop;
using ZweigEngine.Native.OpenGL.Constants;
using ZweigEngine.Native.OpenGL.Prototypes;

namespace ZweigEngine.Native.OpenGL.Renderer;

internal class OpenGLSpriteModel : IDisposable
{
	private const int COMMAND_BUFFER_CAPACITY = 1024;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
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

	// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
	// ReSharper restore InconsistentNaming

	private readonly Command[]               m_commandData;
	private readonly PinnedObject<float[]>   m_quadPinned;
	private readonly PinnedObject<Command[]> m_commandPinned;
	private readonly uint                    m_commandElementSize;
	private          uint                    m_spriteArray;
	private          uint                    m_quadBuffer;
	private          uint                    m_commandBuffer;
	private          uint                    m_commandSize;

	public OpenGLSpriteModel(ICustomFunctionLoader loader)
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

		var quadData = new[]
		{
			0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f,
			1.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f,
			1.0f, 1.0f, 0.0f,
			1.0f, 0.0f, 0.0f,
		};
		var buffers = new uint[2];
		var arrays  = new uint[1];
		glGenBuffers(buffers.Length, buffers);
		glGenVertexArrays(arrays.Length, arrays);

		m_quadBuffer    = buffers[0];
		m_commandBuffer = buffers[1];
		m_spriteArray   = arrays[0];

		if (m_quadBuffer == 0u || m_commandBuffer == 0u)
		{
			throw new Exception("Couldn't allocate sprite vertex buffers.");
		}

		if (m_spriteArray == 0u)
		{
			throw new Exception("Couldn't allocate sprite vertex array.");
		}

		var quadSize = Marshal.SizeOf<float>() * quadData.Length;
		m_quadPinned         = new PinnedObject<float[]>(quadData, GCHandleType.Pinned);
		m_commandData        = new Command[COMMAND_BUFFER_CAPACITY];
		m_commandPinned      = new PinnedObject<Command[]>(m_commandData, GCHandleType.Pinned);
		m_commandSize        = 0;
		m_commandElementSize = (uint)Marshal.SizeOf<Command>();

		var offsetDestination = (UIntPtr)Marshal.OffsetOf<Command>(nameof(Command.Destination));
		var offsetSource      = (UIntPtr)Marshal.OffsetOf<Command>(nameof(Command.Source));
		var offsetTint        = (UIntPtr)Marshal.OffsetOf<Command>(nameof(Command.Tint));
		var offsetFlags       = (UIntPtr)Marshal.OffsetOf<Command>(nameof(Command.Flags));

		glBindBuffer(OpenGLBufferTarget.Array, m_quadBuffer);
		glBufferData(OpenGLBufferTarget.Array, (ulong)quadSize, m_quadPinned.GetAddress(), OpenGLBufferUsage.StaticDraw);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);

		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glBufferData(OpenGLBufferTarget.Array, m_commandElementSize * COMMAND_BUFFER_CAPACITY, m_commandPinned.GetAddress(), OpenGLBufferUsage.StreamDraw);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);

		glBindVertexArray(m_spriteArray);

		glEnableVertexAttribArray(0u);
		glBindBuffer(OpenGLBufferTarget.Array, m_quadBuffer);
		glVertexAttribPointer(0u, 3, OpenGLVertexDataType.Float, false, Marshal.SizeOf<float>() * 3, UIntPtr.Zero);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);

		glEnableVertexAttribArray(1u);
		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glVertexAttribPointer(1u, 4, OpenGLVertexDataType.Int, false, (int)m_commandElementSize, offsetDestination);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);
		glVertexAttribDivisor(1u, 1);

		glEnableVertexAttribArray(2u);
		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glVertexAttribPointer(2u, 4, OpenGLVertexDataType.Int, false, (int)m_commandElementSize, offsetSource);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);
		glVertexAttribDivisor(2u, 1);

		glEnableVertexAttribArray(3u);
		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glVertexAttribPointer(3u, 4, OpenGLVertexDataType.UnsignedByte, false, (int)m_commandElementSize, offsetTint);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);
		glVertexAttribDivisor(3u, 1);

		glEnableVertexAttribArray(4u);
		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glVertexAttribPointer(4u, 4, OpenGLVertexDataType.UnsignedByte, false, (int)m_commandElementSize, offsetFlags);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);
		glVertexAttribDivisor(4u, 1);

		glBindVertexArray(0u);
	}

	private void ReleaseUnmanagedResources()
	{
		if (m_spriteArray != 0u)
		{
			glDeleteVertexArrays(1, new[] { m_spriteArray });
			m_spriteArray = 0u;
		}

		if (m_commandBuffer != 0u)
		{
			glDeleteBuffers(1, new[] { m_commandBuffer });
			m_commandBuffer = 0u;
		}

		if (m_quadBuffer != 0u)
		{
			glDeleteBuffers(1, new[] { m_quadBuffer });
			m_quadBuffer = 0u;
		}

		m_quadPinned.Dispose();
		m_commandPinned.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~OpenGLSpriteModel()
	{
		ReleaseUnmanagedResources();
	}

	public void Begin()
	{
		glBindVertexArray(m_spriteArray);
	}

	public void Finish()
	{
		Flush();
		glBindVertexArray(0u);
	}

	public void Draw(in VideoRect dst, in VideoRect src, in VideoColor tint, VideoBlitFlags blitFlags)
	{
		if (m_commandSize >= COMMAND_BUFFER_CAPACITY)
		{
			Flush();
		}

		ref var cmd = ref m_commandData[m_commandSize];
		cmd.Destination = dst;
		cmd.Source      = src;
		cmd.Tint        = tint;
		cmd.Flags.Red   = blitFlags.HasFlag(VideoBlitFlags.MirrorHorizontal) ? byte.MaxValue : byte.MinValue;
		cmd.Flags.Green = blitFlags.HasFlag(VideoBlitFlags.MirrorVertical) ? byte.MaxValue : byte.MinValue;
		++m_commandSize;
	}

	public void Flush()
	{
		if (m_commandSize == 0)
		{
			return;
		}

		var size = (ulong)(m_commandElementSize * m_commandSize);
		glBindBuffer(OpenGLBufferTarget.Array, m_commandBuffer);
		glBufferData(OpenGLBufferTarget.Array, size, IntPtr.Zero, OpenGLBufferUsage.StreamDraw);
		glBufferData(OpenGLBufferTarget.Array, size, m_commandPinned.GetAddress(), OpenGLBufferUsage.StreamDraw);
		glBindBuffer(OpenGLBufferTarget.Array, 0u);
		glDrawArraysInstanced(OpenGLVertexMode.Triangles, 0, 6u, m_commandSize);
		m_commandSize = 0;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Command
	{
		public VideoRect  Destination;
		public VideoRect  Source;
		public VideoColor Tint;
		public VideoColor Flags;
	}
}
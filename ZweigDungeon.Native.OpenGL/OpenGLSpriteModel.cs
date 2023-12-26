using System.Runtime.InteropServices;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Utility.Interop;
using ZweigDungeon.Native.OpenGL.Constants;
using ZweigDungeon.Native.OpenGL.Handles;
using ZweigDungeon.Native.OpenGL.Resources;

namespace ZweigDungeon.Native.OpenGL;

internal class OpenGLSpriteModel : IDisposable
{
	private const int COMMAND_BUFFER_CAPACITY = 1024;

	private readonly OpenGLArrayManager      m_arrayManager;
	private readonly OpenGLVertexArrayHandle m_spriteArray;

	private readonly OpenGLVertexBufferHandle m_quadBuffer;
	private readonly PinnedObject<float[]>    m_quadPinned;

	private readonly OpenGLVertexBufferHandle m_commandBuffer;
	private readonly Command[]                m_commandData;
	private readonly PinnedObject<Command[]>  m_commandPinned;
	private readonly int                      m_commandElementSize;
	private          int                      m_commandSize;

	public OpenGLSpriteModel(OpenGLArrayManager arrayManager)
	{
		m_arrayManager = arrayManager;

		if (!m_arrayManager.TryCreateVertexBuffer(out m_quadBuffer))
		{
			throw new Exception("Couldn't allocate sprite vertices.");
		}

		var quadData = new[]
		{
			0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f,
			1.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f,
			1.0f, 1.0f, 0.0f,
			1.0f, 0.0f, 0.0f,
		};
		m_quadPinned = new PinnedObject<float[]>(quadData, GCHandleType.Pinned);
		m_arrayManager.InitializeVertexBuffer(m_quadBuffer, Marshal.SizeOf<float>() * quadData.Length, m_quadPinned.GetAddress(), OpenGLBufferUsage.StaticDraw);

		if (!m_arrayManager.TryCreateVertexBuffer(out m_commandBuffer))
		{
			throw new Exception("Couldn't allocate sprite command buffer.");
		}

		m_commandData        = new Command[COMMAND_BUFFER_CAPACITY];
		m_commandPinned      = new PinnedObject<Command[]>(m_commandData, GCHandleType.Pinned);
		m_commandSize        = 0;
		m_commandElementSize = Marshal.SizeOf<Command>();
		m_arrayManager.InitializeVertexBuffer(m_commandBuffer, m_commandElementSize * COMMAND_BUFFER_CAPACITY, m_commandPinned.GetAddress(), OpenGLBufferUsage.StreamDraw);

		if (!m_arrayManager.TryCreateVertexArray(out m_spriteArray))
		{
			throw new Exception("Couldn't allocate sprite model.");
		}

		m_arrayManager.InitializeVertexArray(m_spriteArray, new[]
		{
			new OpenGLVertexAttributeDesc
			{
				IsInstanceParameter = false,
				VertexBuffer        = m_quadBuffer,
				ElementCount        = 3,
				ElementType         = OpenGLVertexDataType.Float,
				Normalize           = false,
				Stride              = Marshal.SizeOf<float>() * 3,
				Offset              = IntPtr.Zero
			},
			new OpenGLVertexAttributeDesc
			{
				IsInstanceParameter = true,
				VertexBuffer        = m_commandBuffer,
				ElementCount        = 4,
				ElementType         = OpenGLVertexDataType.UnsignedInt,
				Normalize           = false,
				Stride              = Marshal.SizeOf<Command>(),
				Offset              = Marshal.OffsetOf<Command>(nameof(Command.Destination))
			},
			new OpenGLVertexAttributeDesc
			{
				IsInstanceParameter = true,
				VertexBuffer        = m_commandBuffer,
				ElementCount        = 4,
				ElementType         = OpenGLVertexDataType.UnsignedInt,
				Normalize           = false,
				Stride              = Marshal.SizeOf<Command>(),
				Offset              = Marshal.OffsetOf<Command>(nameof(Command.Source))
			},
			new OpenGLVertexAttributeDesc
			{
				IsInstanceParameter = true,
				VertexBuffer        = m_commandBuffer,
				ElementCount        = 4,
				ElementType         = OpenGLVertexDataType.UnsignedByte,
				Normalize           = false,
				Stride              = Marshal.SizeOf<Command>(),
				Offset              = Marshal.OffsetOf<Command>(nameof(Command.Tint))
			},
			new OpenGLVertexAttributeDesc
			{
				IsInstanceParameter = true,
				VertexBuffer        = m_commandBuffer,
				ElementCount        = 4,
				ElementType         = OpenGLVertexDataType.UnsignedByte,
				Normalize           = false,
				Stride              = Marshal.SizeOf<Command>(),
				Offset              = Marshal.OffsetOf<Command>(nameof(Command.Flags))
			}
		});
	}

	private void ReleaseUnmanagedResources()
	{
		m_arrayManager.DeleteVertexArray(m_spriteArray);
		m_arrayManager.DeleteVertexBuffer(m_commandBuffer);
		m_arrayManager.DeleteVertexBuffer(m_quadBuffer);
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
		m_arrayManager.BindArray(m_spriteArray);
	}

	public void Finish()
	{
		Flush();
		m_arrayManager.UnbindArray();
	}

	public void Draw(in VideoRect dst, in VideoRect src, in VideoColor tint, in VideoColor flags)
	{
		if (m_commandSize >= COMMAND_BUFFER_CAPACITY)
		{
			Flush();
		}

		ref var cmd = ref m_commandData[m_commandSize];
		cmd.Destination = dst;
		cmd.Source      = src;
		cmd.Tint        = tint;
		cmd.Flags       = flags;
	}

	public void Flush()
	{
		if (m_commandSize == 0)
		{
			return;
		}

		m_arrayManager.UpdateVertexBuffer(m_commandBuffer, m_commandElementSize * m_commandSize, m_commandPinned.GetAddress(), OpenGLBufferUsage.StreamDraw);
		m_arrayManager.DrawArrayInstance(m_spriteArray, OpenGLVertexMode.Triangles, 0, 6, m_commandSize);
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
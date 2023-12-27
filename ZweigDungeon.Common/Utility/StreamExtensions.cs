using System.Runtime.InteropServices;

namespace ZweigDungeon.Common.Utility;

public static class StreamExtensions
{
	public static async Task<TStruct> ReadStructureAsync<TStruct>(this Stream reader, CancellationToken cancellationToken) where TStruct : struct
	{
		var size   = Marshal.SizeOf(typeof(TStruct));
		var buffer = new byte[size];
		var read   = await reader.ReadAsync(buffer, cancellationToken);

		if (read != buffer.Length)
		{
			throw new IOException("Failed to read sufficient bytes for structure.");
		}

		var pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);

		try
		{
			return (TStruct)Marshal.PtrToStructure(pinned.AddrOfPinnedObject(), typeof(TStruct))!;
		}
		finally
		{
			if (pinned.IsAllocated)
			{
				pinned.Free();
			}
		}
	}
}
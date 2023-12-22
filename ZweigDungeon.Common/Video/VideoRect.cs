using System.Runtime.InteropServices;

namespace ZweigDungeon.Common.Video;

[StructLayout(LayoutKind.Sequential)]
public struct VideoRect
{
	public uint Left;
	public uint Top;
	public uint Width;
	public uint Height;
}
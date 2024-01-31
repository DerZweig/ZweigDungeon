using System.Runtime.InteropServices;

namespace ZweigEngine.Common.Services.Interfaces.Video.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct VideoRect
{
	public int Left;
	public int Top;
	public int Width;
	public int Height;

	public bool Intersects(in VideoRect other)
	{
		return Left < other.Left + other.Width && 
		       Top < other.Top + other.Height &&
		       other.Left < Left + Width && 
		       other.Top < Top + Height;
	}
}
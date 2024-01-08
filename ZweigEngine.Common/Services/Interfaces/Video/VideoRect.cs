using System.Runtime.InteropServices;

namespace ZweigEngine.Common.Services.Interfaces.Video;

[StructLayout(LayoutKind.Sequential)]
public struct VideoRect
{
	public int Left;
	public int Top;
	public int Width;
	public int Height;
}
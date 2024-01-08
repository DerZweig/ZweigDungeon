using System.Runtime.InteropServices;

namespace ZweigEngine.Common.Services.Interfaces.Video;

[StructLayout(LayoutKind.Sequential)]
public struct VideoColor
{
	public byte Red;
	public byte Green;
	public byte Blue;
	public byte Alpha;
}
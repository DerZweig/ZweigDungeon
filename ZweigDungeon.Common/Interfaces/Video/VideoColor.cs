using System.Runtime.InteropServices;

namespace ZweigDungeon.Common.Interfaces.Video;

[StructLayout(LayoutKind.Sequential)]
public struct VideoColor
{
	public byte Red;
	public byte Green;
	public byte Blue;
	public byte Alpha;
}
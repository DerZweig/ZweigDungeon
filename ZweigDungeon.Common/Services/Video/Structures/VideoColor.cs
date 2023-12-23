using System.Runtime.InteropServices;

namespace ZweigDungeon.Common.Services.Video.Structures;

[StructLayout(LayoutKind.Sequential)]
public struct VideoColor
{
	public byte Red;
	public byte Green;
	public byte Blue;
	public byte Alpha;
}
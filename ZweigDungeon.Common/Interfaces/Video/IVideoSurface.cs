namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoSurface : IDisposable
{
	ushort Width  { get; }
	ushort Height { get; }
}
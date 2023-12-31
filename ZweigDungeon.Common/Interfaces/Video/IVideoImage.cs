namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoImage : IDisposable
{
	ushort Width  { get; }
	ushort Height { get; }
}
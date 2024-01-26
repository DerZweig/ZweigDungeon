namespace ZweigDungeon.Application.Services.Interfaces;

public interface IGlobalTimers
{
	DateTime CurrentTimeUniversal { get; }
	DateTime CurrentTimeLocal     { get; }
	TimeSpan FrameTimeDelta       { get; }
	TimeSpan TotalRunTime         { get; }
}
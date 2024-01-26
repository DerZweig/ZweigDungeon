using ZweigDungeon.Application.Services.Interfaces;

namespace ZweigDungeon.Application.Services.Implementation;

public class GlobalTimers : IGlobalTimers
{
	public void Reset()
	{
		var now = DateTime.UtcNow;
		CurrentTimeUniversal = now;
		CurrentTimeLocal     = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.Local);
		FrameTimeDelta       = TimeSpan.Zero;
		TotalRunTime         = TimeSpan.Zero;
	}

	public void Update()
	{
		var now      = DateTime.UtcNow;
		var previous = CurrentTimeUniversal;
		var delta    = now - previous;

		CurrentTimeUniversal =  now;
		CurrentTimeLocal     =  TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.Local);
		FrameTimeDelta       =  delta;
		TotalRunTime         += delta;
	}

	public DateTime CurrentTimeUniversal { get; private set; }
	public DateTime CurrentTimeLocal     { get; private set; }
	public TimeSpan FrameTimeDelta       { get; private set; }
	public TimeSpan TotalRunTime         { get; private set; }
}
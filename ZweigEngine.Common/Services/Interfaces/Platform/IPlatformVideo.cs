namespace ZweigEngine.Common.Services.Interfaces.Platform;

public interface IPlatformVideo
{
	string Name { get; }
	int    GetViewportWidth();
	int    GetViewportHeight();
}
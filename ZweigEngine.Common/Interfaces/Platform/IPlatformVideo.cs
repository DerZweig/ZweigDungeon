namespace ZweigEngine.Common.Interfaces.Platform;

public interface IPlatformVideo
{
	string Name { get; }
	int    GetViewportWidth();
	int    GetViewportHeight();
}
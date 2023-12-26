namespace ZweigDungeon.Common.Interfaces.Video;

public interface IVideoContext
{
	void BeginFrame(int viewportWidth, int viewportHeight);
	void FinishFrame();
}
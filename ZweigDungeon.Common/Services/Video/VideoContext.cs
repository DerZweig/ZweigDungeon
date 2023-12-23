using ZweigDungeon.Common.Services.Video.Descriptors;
using ZweigDungeon.Common.Services.Video.Resources;

namespace ZweigDungeon.Common.Services.Video;

public abstract class VideoContext
{
	protected VideoContext()
	{
		
	}

	public abstract bool CreateShader(in VideoPixelShaderDescription desc, out VideoShader shader);

	public abstract bool CreateRenderTarget(in VideoRenderTargetDescription desc, out VideoRenderTarget renderTarget);

	public abstract bool CreateTexture2D(in VideoTexture2DDescription desc, out VideoTexture2D texture2D);

	public abstract bool CreateVertexBuffer(in VideoVertexBufferDescription desc, out VideoVertexBuffer buffer);
	
	public abstract void Delete(AbstractVideoResource resource);
}
using ZweigDungeon.Common.Assets.Font;
using ZweigDungeon.Common.Interfaces.Platform;
using ZweigDungeon.Common.Interfaces.Platform.Messages;
using ZweigDungeon.Common.Interfaces.Video;
using ZweigDungeon.Common.Services.Messages;

namespace ZweigDungeon.Application.Manager;

public sealed class FontManager : IDisposable, IWindowListener
{
	private readonly MessageBus              m_messageBus;
	private readonly IVideoContext           m_videoContext;
	private readonly IDisposable             m_subscription;
	private readonly CancellationTokenSource m_cancellationTokenSource;
	private          Task?                   m_loading;

	public FontManager(MessageBus messageBus, IVideoContext videoContext)
	{
		m_messageBus              = messageBus;
		m_videoContext            = videoContext;
		m_subscription            = messageBus.Subscribe<IWindowListener>(this);
		m_cancellationTokenSource = new CancellationTokenSource();
	}

	private void ReleaseUnmanagedResources()
	{
		m_subscription.Dispose();
		m_cancellationTokenSource.Dispose();
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~FontManager()
	{
		ReleaseUnmanagedResources();
	}

	public void WindowCreated(IPlatformWindow window)
	{
		m_loading = InitializeFonts(m_cancellationTokenSource.Token);
	}

	public void WindowClosing(IPlatformWindow window)
	{
		m_cancellationTokenSource.Cancel();
	}

	public async void WindowUpdateFrame(IPlatformWindow window)
	{
		if (m_loading is { IsCompleted: true })
		{
			await m_loading;
			m_loading = null;
		}
	}

	private async Task<IReadOnlyList<FontDefinition>> InitializeFonts(CancellationToken cancellationToken)
	{
		var filesInDirectory = Directory.EnumerateFiles("Data/Fonts").ToArray();
		var fontDescriptions = filesInDirectory.Where(HasFilePathFontExtension).ToArray();
		var reader           = new FontScriptReader();
		var definitions      = new List<FontDefinition>();
		
		foreach (var filePath in fontDescriptions)
		{
			await using (var stream = File.OpenRead(filePath))
			{
				var definition = await reader.LoadContents(stream, cancellationToken);
				if (definition == null)
				{
					continue;
				}
				
				definitions.Add(definition);
			}
		}

		return definitions;
	}

	private static bool HasFilePathFontExtension(string filePath)
	{
		return string.Equals(Path.GetExtension(filePath), ".fnt", StringComparison.OrdinalIgnoreCase);
	}
}
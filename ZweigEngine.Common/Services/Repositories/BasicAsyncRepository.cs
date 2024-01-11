using ZweigEngine.Common.Services.Repositories.Constants;
using ZweigEngine.Common.Utility.Concurrency;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigEngine.Common.Services.Repositories;

public abstract class BasicAsyncRepository<TAsset>
{
	private readonly ExclusiveTaskFactory      m_synchronization;
	private readonly Dictionary<string, Entry> m_entries;

	protected BasicAsyncRepository()
	{
		m_synchronization = new ExclusiveTaskFactory();
		m_entries         = new Dictionary<string, Entry>();
	}

	protected Task Synchronize(Action work, CancellationToken cancellationToken) => m_synchronization.Invoke(work, cancellationToken);

	protected Task<TResult> Synchronize<TResult>(Func<TResult> work, CancellationToken cancellationToken) => m_synchronization.Invoke(work, cancellationToken); 

	public Task ClearScene(CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		var kvs = m_entries.ToArray();
		foreach (var pair in kvs)
		{
			if (pair.Value.Scope == AssetScope.Scene)
			{
				m_entries.Remove(pair.Key);
			}
		}
	}, cancellationToken);

	public Task ClearSession(CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		var kvs = m_entries.ToArray();
		foreach (var pair in kvs)
		{
			if (pair.Value.Scope == AssetScope.Session || pair.Value.Scope == AssetScope.Scene)
			{
				m_entries.Remove(pair.Key);
			}
		}
	}, cancellationToken);

	public Task LoadForGlobal(string path, CancellationToken cancellationToken) => LoadAsset(path, AssetScope.Global, cancellationToken);

	public Task LoadForSession(string path, CancellationToken cancellationToken) => LoadAsset(path, AssetScope.Session, cancellationToken);

	public Task LoadForScene(string path, CancellationToken cancellationToken) => LoadAsset(path, AssetScope.Scene, cancellationToken);

	private Task LoadAsset(string path, AssetScope scope, CancellationToken cancellationToken) => m_synchronization.Invoke(async () =>
	{
		var normalizedName = path.Trim().ToLower();
		if (m_entries.TryGetValue(normalizedName, out var entry))
		{
			if (entry.Scope != scope)
			{
				switch (entry.Scope)
				{
					case AssetScope.Scene when scope == AssetScope.Session:
					case AssetScope.Session when scope == AssetScope.Global:
						entry.Scope = scope;
						break;
					case AssetScope.Global:
						break;
					default:
						throw new UnhandledEnumException<AssetScope>(scope);
				}
			}
			
			if (entry.Pending != null)
			{
				await entry.Pending;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
		else
		{
			entry = new Entry { Scope = scope };
			m_entries.Add(normalizedName, entry);
			try
			{
				var worker = LoadContents(path, cancellationToken);
				entry.Pending = worker;
				entry.Asset   = await worker;
			}
			finally
			{
				entry.Pending = null;
			}
		}

		return entry.Asset ?? throw new FileLoadException($"Couldn't load asset {path} of type {typeof(TAsset).Name}");
	}, cancellationToken);
	
	protected abstract Task<TAsset> LoadContents(string path, CancellationToken cancellationToken);

	private class Entry
	{
		public Task?      Pending { get; set; }
		public TAsset?    Asset   { get; set; }
		public AssetScope Scope   { get; set; }
	}
}
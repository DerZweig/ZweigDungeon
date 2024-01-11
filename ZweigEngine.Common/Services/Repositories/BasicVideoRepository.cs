using ZweigEngine.Common.Services.Interfaces.Platform;
using ZweigEngine.Common.Services.Repositories.Constants;
using ZweigEngine.Common.Utility.Exceptions;

namespace ZweigEngine.Common.Services.Repositories;

public abstract class BasicVideoRepository<TAsset, TResource> : IDisposable where TAsset : class
                                                                            where TResource : class
{
	private readonly IPlatformSynchronization  m_synchronization;
	private readonly Dictionary<TAsset, Entry> m_entries;

	protected BasicVideoRepository(IPlatformSynchronization synchronization)
	{
		m_synchronization = synchronization;
		m_entries         = new Dictionary<TAsset, Entry>();
	}

	private void ReleaseUnmanagedResources()
	{
		var resources = m_entries.Values.Select(x => x.Resource).ToArray();
		m_entries.Clear();
		foreach (var resource in resources)
		{
			if (resource is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~BasicVideoRepository()
	{
		ReleaseUnmanagedResources();
	}

	public Task ClearScene(CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		var kvs       = m_entries.ToArray();
		var resources = new List<TResource?>();
		foreach (var pair in kvs)
		{
			if (pair.Value.Scope == AssetScope.Scene)
			{
				m_entries.Remove(pair.Key);
				resources.Add(pair.Value.Resource);
			}
		}

		foreach (var resource in resources)
		{
			if (resource is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}, cancellationToken);

	public Task ClearSession(CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		var kvs       = m_entries.ToArray();
		var resources = new List<TResource?>();
		foreach (var pair in kvs)
		{
			if (pair.Value.Scope == AssetScope.Session || pair.Value.Scope == AssetScope.Scene)
			{
				m_entries.Remove(pair.Key);
				resources.Add(pair.Value.Resource);
			}
		}

		foreach (var resource in resources)
		{
			if (resource is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}, cancellationToken);

	public Task LoadForGlobal(TAsset asset, CancellationToken cancellationToken) => CreateResource(asset, AssetScope.Global, cancellationToken);

	public Task LoadForSession(TAsset asset, CancellationToken cancellationToken) => CreateResource(asset, AssetScope.Session, cancellationToken);

	public Task LoadForScene(TAsset asset, CancellationToken cancellationToken) => CreateResource(asset, AssetScope.Scene, cancellationToken);

	private Task CreateResource(TAsset asset, AssetScope scope, CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		if (m_entries.TryGetValue(asset, out var entry))
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

			return;
		}

		entry = new Entry
		{
			Resource = CreateResource(asset),
			Scope    = scope
		};

		m_entries.Add(asset, entry);
		UploadResource(entry.Resource, asset);
	}, cancellationToken);

	public Task<bool> TryUpload(TAsset asset, CancellationToken cancellationToken) => m_synchronization.Invoke(() =>
	{
		if (!m_entries.TryGetValue(asset, out var entry))
		{
			return false;
		}

		UploadResource(entry.Resource, asset);
		return true;
	}, cancellationToken);

	public void BindOrIgnore(TAsset asset, Action<TResource> work) => m_synchronization.ExecuteWithGuard(() =>
	{
		if (m_entries.TryGetValue(asset, out var entry))
		{
			work(entry.Resource);
		}
	});

	protected abstract TResource CreateResource(TAsset asset);
	protected abstract void      UploadResource(TResource resource, TAsset asset);

	private class Entry
	{
		public TResource  Resource { get; init; } = default!;
		public AssetScope Scope    { get; set; }
	}
}
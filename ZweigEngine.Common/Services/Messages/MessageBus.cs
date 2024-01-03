using System.Collections.Concurrent;

namespace ZweigEngine.Common.Services.Messages;

public sealed class MessageBus 
{
	private readonly ConcurrentDictionary<Type, object> m_containers;

	public MessageBus()
	{
		m_containers = new ConcurrentDictionary<Type, object>();
	}

	public IDisposable Subscribe<TInterface>(TInterface handler) where TInterface : class
	{
		var subscribers = m_containers.GetOrAdd(typeof(TInterface), _ => new MessageContainer<TInterface>());
		return ((MessageContainer<TInterface>)subscribers).Insert(handler);
	}

	public void Broadcast<TInterface>(in Action<TInterface> work) where TInterface : class
	{
		if (m_containers.TryGetValue(typeof(TInterface), out var subscribers))
		{
			((MessageContainer<TInterface>)subscribers).Invoke(work);
		}
	}
}
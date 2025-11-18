using ZweigEngine.Common.Services;
using ZweigEngine.Common.Utility;

namespace ZweigEngine.Platform.SDL;

internal sealed class SDLPlatformBase : DisposableObject
{
    private readonly ILogger                 m_logger;
    private readonly List<ISDLEventListener> m_listeners;

    private bool m_init;

    public SDLPlatformBase(ILogger logger)
    {
        m_logger    = logger;
        m_listeners = new List<ISDLEventListener>();
    }

    protected override void ReleaseUnmanagedResources()
    {
        if (!m_init)
        {
            return;
        }

        m_logger.Info(nameof(SDL3.SDL), "Shutdown");
        SDL3.SDL.Quit();
    }

    public bool Initialize(string title, string version, string identifier)
    {
        if (m_init)
        {
            m_logger.Error(nameof(SDL3.SDL), "Already initialized");
            return false;
        }

        m_logger.Info(nameof(SDL3.SDL), "Initialize SDL");
        if (!SDL3.SDL.SetAppMetadata(title, version, identifier) ||
            !SDL3.SDL.Init(SDL3.SDL.InitFlags.Video | SDL3.SDL.InitFlags.Audio))
        {
            m_logger.Error(nameof(SDL3.SDL), $"Failed to initialize SDL {SDL3.SDL.GetError()}");
            return false;
        }

        m_init = true;
        return true;
    }

    public void Subscribe(ISDLEventListener listener)
    {
        m_listeners.Add(listener);
    }

    public void Unsubscribe(ISDLEventListener listener)
    {
        m_listeners.Remove(listener);
    }

    public bool ProcessInput(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        foreach (var listener in m_listeners)
        {
            listener.InputBegin();
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!SDL3.SDL.PollEvent(out var ev))
            {
                break;
            }

            if (ev.Type == (uint)SDL3.SDL.EventType.Quit)
            {
                return false;
            }

            foreach (var listener in m_listeners)
            {
                listener.InputMessage(ev);
            }
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        foreach (var listener in m_listeners)
        {
            listener.InputFinish();
        }

        return !cancellationToken.IsCancellationRequested;
    }
}
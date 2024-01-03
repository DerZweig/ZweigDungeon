namespace ZweigEngine.Common.Utility.Concurrency;

public struct ConcurrentCounter
{
    private long m_location;

    public ConcurrentCounter(long value)
    {
        m_location = value;
    }

    public long Read()
    {
        return Interlocked.Read(ref m_location);
    }

    public long Increment()
    {
        return Interlocked.Increment(ref m_location);
    }

    public long Decrement()
    {
        return Interlocked.Decrement(ref m_location);
    }
}
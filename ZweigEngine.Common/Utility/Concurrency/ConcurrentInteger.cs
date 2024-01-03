namespace ZweigEngine.Common.Utility.Concurrency;

public struct ConcurrentInteger
{
    private long m_location;

    public ConcurrentInteger(long value)
    {
        m_location = value;
    }

    public long Read()
    {
        return Interlocked.Read(ref m_location);
    }

    public long Exchange(long value)
    {
        return Interlocked.Exchange(ref m_location, value);
    }

    public long CompareExchange(long value, long comparand)
    {
        return Interlocked.CompareExchange(ref m_location, value, comparand);
    }
}
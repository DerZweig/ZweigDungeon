namespace ZweigEngine.Common.Utility.Concurrency;

public struct ConcurrentObject<TValue> where TValue : class
{
    private TValue? m_location;

    public ConcurrentObject(TValue? value)
    {
        m_location = value;
    }

    public TValue? Read()
    {
        return Interlocked.CompareExchange(ref m_location, null, null);
    }

    public TValue? Exchange(TValue? value)
    {
        return Interlocked.Exchange(ref m_location, value);
    }

    public TValue? CompareExchange(TValue? value, TValue? comparand)
    {
        return Interlocked.CompareExchange(ref m_location, value, comparand);
    }
}
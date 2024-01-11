namespace ZweigEngine.Common.Assets.Font;

public readonly struct FontKerning
{
	private readonly char m_first;
	private readonly char m_second;

	public FontKerning(char first, char second)
	{
		m_first  = first;
		m_second = second;
	}

	public bool Equals(FontKerning other)
	{
		return m_first == other.m_first && m_second == other.m_second;
	}

	public override bool Equals(object? obj)
	{
		return obj is FontKerning other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_first, m_second);
	}

	public static bool operator ==(FontKerning left, FontKerning right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(FontKerning left, FontKerning right)
	{
		return !left.Equals(right);
	}
}
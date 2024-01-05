namespace ZweigEngine.Common.Utility.Exceptions;

public class EnumOutOfRangeException<TEnum> : Exception where TEnum : Enum
{
	public EnumOutOfRangeException(TEnum value) : base($"Enumeration {value} of type {typeof(TEnum).Name} is out of range.")
	{

	}
}
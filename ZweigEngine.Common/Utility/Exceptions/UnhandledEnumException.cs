namespace ZweigEngine.Common.Utility.Exceptions;

public class UnhandledEnumException<TEnum> : Exception where TEnum : Enum
{
	public UnhandledEnumException(TEnum value) : base($"Enumeration {value} of type {typeof(TEnum).Name} was not handled.")
	{

	}
}
namespace ZweigEngine.Common.Services;

public interface IGlobalCancellation
{
    CancellationToken Token { get; }
}
using System.Threading;

namespace ZweigDungeon.Application.Services.Interfaces;

public interface IGlobalCancellation
{
	CancellationToken Token { get; }
}
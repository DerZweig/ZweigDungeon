namespace ZweigEngine.Common.Services;

public interface ILogger
{
    void Log(LogLevel level, string location, string message);

    void Info(string location, string message)
    {
        Log(LogLevel.Info, location, message);
    }

    void Warning(string location, string message)
    {
        Log(LogLevel.Warning, location, message);
    }

    void Error(string location, string message)
    {
        Log(LogLevel.Error, location, message);
    }
}
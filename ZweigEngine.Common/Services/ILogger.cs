using System.Text;

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

    void Error(string location, string message, Exception ex)
    {
        var builder = new StringBuilder();
        builder.AppendLine(message);
        builder.AppendLine(ex.ToString());
        Log(LogLevel.Error, location, builder.ToString());
    }
}
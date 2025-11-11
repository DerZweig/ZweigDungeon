using System.Collections.Concurrent;
using System.Diagnostics;
using ZweigEngine.Common.Services;
using ZweigEngine.Common.Utility;

namespace ZweigDungeon.Services;

internal sealed class FileLogger : DisposableObject, ILogger
{
    private readonly BlockingCollection<string> m_queue;
    private readonly Task                       m_worker;
    private readonly StreamWriter               m_writer;
    private readonly TimeSpan                   m_timeout;

    public FileLogger(string filePath, TimeSpan timeout)
    {
        m_timeout = timeout;
        m_queue   = new BlockingCollection<string>();
        m_worker  = Task.Factory.StartNew(Process, this, TaskCreationOptions.LongRunning);
        m_writer  = File.CreateText(filePath);
    }

    protected override void ReleaseUnmanagedResources()
    {
        try
        {
            m_queue.CompleteAdding();
            m_worker.Wait(m_timeout);
        }
        finally
        {
            m_writer.Close();
        }
    }

    private static void Process(object? obj)
    {
        var self = (FileLogger)obj!;
        try
        {
            foreach (var entry in self.m_queue.GetConsumingEnumerable())
            {
                self.m_writer.WriteLine(entry);
            }
        }
        catch
        {
            //ignore
        }
    }

    public void Log(LogLevel level, string location, string message)
    {
        var text = $"{DateTime.Now} [{level}] {location} - {message}";
        if (Debugger.IsAttached)
        {
            Debug.WriteLine(text);
        }

        m_queue.Add(text);
    }
}
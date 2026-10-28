namespace Core.Notifications;

using System.IO;
using System;
using System.Collections.Generic;

public interface INotificationSink
{
    void Send(string message);
}

// Вивід у консоль
public class ConsoleSink : INotificationSink
{
    public void Send(string message) => Console.WriteLine(message);
}

// Вивід у файловий лог
public class FileSink : INotificationSink
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");

    public void Send(string message)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.AppendAllText(_path, message + Environment.NewLine);
    }
}

// Композитний синк (щоб слати і в консоль, і у файл одночасно)
public class CompositeSink(IEnumerable<INotificationSink> sinks) : INotificationSink
{
    private readonly IEnumerable<INotificationSink> _sinks = sinks;

    public void Send(string message)
    {
        foreach (var sink in _sinks)
        {
            sink.Send(message);
        }
    }
}
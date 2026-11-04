namespace Core.Notifications;

using System.IO;
using System;
using System.Collections.Generic;

// Загальний інтерфейс для приймачів сповіщень ( Sink )
public interface INotificationSink
{
    void Send(string message);
}

// Реалізація 1: виводить сповіщення в консоль
public class ConsoleSink : INotificationSink
{
    public void Send(string message) => Console.WriteLine(message);
}

// Реалізація 2: записує сповіщення у файл логів
public class FileSink : INotificationSink
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");

    public void Send(string message)
    {
        // Автоматично створюємо панку logs, якщо її немає
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        // Дописуємо рядок у кінець файлу
        File.AppendAllText(_path, message + Environment.NewLine);
    }
}

// Композитний синк (дозволяє передати одразу кілька приймачів, наприклад і консоль, і файл)
public class CompositeSink(IEnumerable<INotificationSink> sinks) : INotificationSink
{
    private readonly IEnumerable<INotificationSink> _sinks = sinks;

    public void Send(string message)
    {
        foreach (var sink in _sinks)
        {
            sink.Send(message); // Передаємо повідомлення кожному синку з колекції
        }
    }
}
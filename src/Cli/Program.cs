using Core.Abstractions;
using Core.Storage;
using Core.Services;
using Core.Events;
using Core.Notifications;
using System;

ICatalogStore store = StoreFactory.Create(args);

// Створюємо комбінований синк (консоль + файл) і передаємо через конструктор (ручний DI)
var notificationSink = new CompositeSink(new INotificationSink[]
{
    new ConsoleSink(),
    new FileSink()
});

var service = new CatalogService(store, notificationSink);

Console.WriteLine($"Сховище налаштовано: {store.GetType().Name}\n");

int changes = 0;
service.Changed += (_, _) => changes++;

var created = service.Add("SKU-101", "Кабель UTP cat6", "м", 50);
service.Receive(created.Id, 25);

Console.WriteLine($"\nПодій отримано через лямбду: {changes}");
namespace EventBus.Demo.V2;

public class EventBus
{
    public static void Test()
    {
        Publisher publisher = new Publisher();
        Subscriber subscriber = new Subscriber();

        publisher.PublisherHandlerEvent += subscriber.Subscribe;
        publisher.Publish(new EventData() { EventId = Guid.NewGuid().ToString("N"), EventName = "EventName", Data = "EventData" });
    }
}

public interface IEventData
{
    string? EventId { get; set; }
    string? EventName { get; set; }
    DateTime EventDateTime { get; set; }
    string? Data { get; set; }
}

public class EventData : IEventData
{
    public string? EventId { get; set; }
    public string? EventName { get; set; }
    public DateTime EventDateTime { get; set; }
    public string? Data { get; set; }

    public EventData()
    {
        EventDateTime = DateTime.Now;
    }
}

public class Publisher
{
    public delegate void PublisherHandler(IEventData? eventData);

    public event PublisherHandler? PublisherHandlerEvent;

    public void Publish(IEventData? publisherHandler)
    { PublisherHandlerEvent?.Invoke(publisherHandler); }
}

public class Subscriber
{
    public void Subscribe(IEventData? subscriber)
    {
        System.Console.WriteLine("V2 版本订阅");
        System.Console.WriteLine($"EventId: {subscriber?.EventId}, EventName: {subscriber?.EventName}, EventDateTime: {subscriber?.EventDateTime}, Data: {subscriber?.Data}");
    }
}


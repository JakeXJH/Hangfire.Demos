using EventBus.Demo.V2;
using System.Reflection;

namespace EventBus.Demo.V3;

public class ConputerService
{
    private Publisher _Publisher;

    public ConputerService()
    {
        _Publisher = new Publisher();
    }

    /// <summary>
    /// 开机中
    /// </summary>
    public void Starting()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}--> 电脑开机中...");
        Task.Delay(2000).Wait();//模拟开机花费时间
        //场景模拟，找出当前系统中的服务
        Assembly assembly = Assembly.GetExecutingAssembly();

        //将所有绑定了的订阅者，添加到订阅中心
        var subscribeMethods = assembly.GetTypes()
          .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
          .Where(method => method.GetCustomAttributes(typeof(SubscribeAttribute), false).Any());

        foreach (var subscribeMethod in subscribeMethods)
        {
            foreach (SubscribeAttribute att in subscribeMethod.GetCustomAttributes(typeof(SubscribeAttribute), false))
            {
                var key = att.SubscriberName;
                QueueDemo.AddSubscribes(key, subscribeMethod);
            }
        }
    }

    /// <summary>
    /// 开机完成
    /// </summary>
    public void Started()
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->电脑开机完成，发布系统通知，处理开机后续任务");
        //CpmStart，电脑开机完成消息标识，只有订阅了这个消息的订阅者，才能收到消息
        _Publisher.Publish("CpmStart", new EventData() { EventId = Guid.NewGuid().ToString("N"), EventName = "CpmStart", Data = "电脑开机完成" });
    }
}

/// <summary>
/// QQ服务
/// </summary>
public class QQService
{
    [Subscribe("CpmStart")]
    public void Run(IEventData eventData)
    {
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->QQ启动中...");
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->QQ启动完成，EventId: {eventData.EventId}, EventName: {eventData.EventName}, EventDateTime: {eventData.EventDateTime}, Data: {eventData.Data}");
    }
}

/// <summary>
/// 微信服务
/// </summary>
public class WXService
{
    [Subscribe("CpmStart")]
    public void Run(IEventData eventData)
    {
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->微信启动中...");
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->QQ启动完成，EventId: {eventData.EventId}, EventName: {eventData.EventName}, EventDateTime: {eventData.EventDateTime}, Data: {eventData.Data}");
    }
}

/// <summary>
/// 腾讯视频服务
/// </summary>
public class TxspService
{
    [Subscribe("QQStart")]
    public void Run(IEventData eventData)
    {
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->腾讯视频启动中...");
        System.Console.WriteLine($"{DateTime.Now:HH:mm:ss:ffff}-->QQ启动完成，EventId: {eventData.EventId}, EventName: {eventData.EventName}, EventDateTime: {eventData.EventDateTime}, Data: {eventData.Data}");
    }
}

/// <summary>
/// 代理人（消息队列）
/// </summary>
public class QueueDemo
{
    private static CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// 消息中心，用于任务添加和消费 key：队列名称 value：队列消息
    /// </summary>
    private static Dictionary<string, Queue<IEventData>> Queues = [];

    /// <summary>
    /// 订阅中心，用于订阅者添加 key：队列名称 value:事务订阅着
    /// </summary>
    private static Dictionary<string, List<MethodInfo>> Subscribes = [];

    /// <summary>
    /// 将消息发送到队列
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="data"></param>
    public static void EnQueue(string queueName, IEventData data)
    {
        if (!Queues.ContainsKey(queueName))
        {
            Queues.Add(queueName, new Queue<IEventData>());

            CreateTask(queueName);
        }
        //将消息发送到队列
        Queues[queueName]?.Enqueue(data);
    }

    /// <summary>
    /// 添加订阅者
    /// </summary>
    /// <param name="key"></param>
    /// <param name="method"></param>
    public static void AddSubscribes(string key, MethodInfo method)
    {
        if (!Subscribes.ContainsKey(key))
        {
            Subscribes.Add(key, new List<MethodInfo>());
        }

        Subscribes[key].Add(method);
    }

    /// <summary>
    /// 线程监视队列，当有消息时，执行订阅者的方法
    /// </summary>
    /// <param name="key"></param>
    private static void CreateTask(string key)
    {
        Task task = new(() =>
        {
            var queName = key;
            while (true)
            {
                if (Queues[queName].Count() == 0) continue;
                var data = Queues[queName]?.Dequeue();
                if (data == null)
                {
                    Task.Delay(100).Wait();
                    continue;
                }

                if (!Subscribes.ContainsKey(key)) continue;
                var methods = Subscribes[key];

                Parallel.ForEach(methods, (method) =>
                {
                    object? instance = Activator.CreateInstance(method.DeclaringType);
                    method.Invoke(instance, [data]);
                });
            }
        }, cancellationTokenSource.Token);

        task.Start();
    }
}

//暴露给外部操作胡类
public class Publisher
{
    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="queueKey">消息队列名称</param>
    /// <param name="eventData">消息体</param>
    public void Publish(string queueKey, V2.IEventData eventData)
    {
        QueueDemo.EnQueue(queueKey, eventData);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class SubscribeAttribute : Attribute
{
    public string SubscriberName { get; set; }

    public SubscribeAttribute(string subscriberName)
    {
        SubscriberName = subscriberName;
    }
}
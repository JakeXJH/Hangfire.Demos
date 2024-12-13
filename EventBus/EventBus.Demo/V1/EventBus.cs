namespace EventBus.Demo.V1
{
    // 旨在体现事件的发布与订阅的过程，而不是事件的触发与处理的过程

    /// <summary>
    /// 事件V1版本
    /// </summary>
    public class EventBus
    {
        public static void Test()
        {
            //创建发布者
            Publisher publisher = new Publisher();
            //创建订阅者
            Subscriber subscriber = new Subscriber();
            //订阅
            subscriber.Publisher = publisher;

            publisher.PublishHandlerEvent += subscriber.Subscribe;

            //发布，无参数发布
            publisher.Publish();

            //发布，有参数发布
            publisher.Publish($"EventId: {Random.Shared.Next().ToString()}", "EventParam: Random Param");
        }
    }

    //发布者
    public class Publisher
    {
        public delegate void PublishHandler(string name, string param);

        public PublishHandler? PublishHandlerEvent;
        /// <summary>
        /// 无参数发布
        /// </summary>
        public void Publish()
        {
            Console.WriteLine("this is publisher.");
            PublishHandlerEvent?.Invoke(Guid.NewGuid().ToString("N"), "Random Event: " + Random.Shared.Next().ToString());
        }

        /// <summary>
        /// 模仿CAP事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        public void Publish(string name, string param)
        {
            PublishHandlerEvent?.Invoke(name, param);
        }
    }

    public class Subscriber
    {
        public string? Name { get; set; }

        public Publisher? Publisher { get; set; }

        public void Subscribe(string name, string param)
        {
            System.Console.WriteLine($"V1 版本订阅");
            Console.WriteLine($"{Name}执行==>: {name}");
            Console.WriteLine($"CallBak==> {param}");
        }
    }
}
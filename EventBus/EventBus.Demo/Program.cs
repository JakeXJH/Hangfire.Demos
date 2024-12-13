using EventBus.Demo.V3;

namespace EventBus.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConputerService conputerService = new ConputerService();
            conputerService.Starting(); ;
            conputerService.Started();
            Console.ReadLine();
        }
    }
}
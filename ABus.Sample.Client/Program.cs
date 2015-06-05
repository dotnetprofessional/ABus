using System;
 
namespace ABus.Sample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Client started - press enter to send messages");
            Console.ReadLine();
            var bus = new Pipeline().StartUsingConfigureHost();
            {
                bus.Trace.Information("About to send some messages!!!");

                do
                {
                    bus.Send(new TestMessageCommand());

                } while (Console.ReadLine() != "x");
            }
        }
    }
}

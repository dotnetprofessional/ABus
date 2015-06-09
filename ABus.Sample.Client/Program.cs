using System;
using ABus.Sample.Contracts.Payments;

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
                    // Using CustomerBC
                    bus.Send(new TestMessageCommand());

                    // Using PaymentsBC
                    bus.Send(new MakePaymentCommand());

                } while (Console.ReadLine() != "x");
            }
        }
    }
}

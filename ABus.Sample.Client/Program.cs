using System;
using System.Diagnostics;

namespace ABus.Sample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleTracer = new ColorConsoleTraceListener();
            Trace.Listeners.Add(consoleTracer);
            var bus = Pipeline.StartUsingConfigureHost();
            {
                bus.Send(new TestMessageCommand());
            }

            Console.ReadLine();
        }
    }
}

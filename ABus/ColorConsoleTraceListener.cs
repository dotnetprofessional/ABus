using System;
using System.Collections.Generic;
using System.Diagnostics;
 
namespace ABus
{
    public class ColorConsoleTraceListener : TraceListener
    {
        readonly Dictionary<TraceEventType, ConsoleColor> eventColor = new Dictionary<TraceEventType, ConsoleColor>();

        public ColorConsoleTraceListener()
        {
            this.InitializeColors();
        }

        void InitializeColors()
        {
            this.eventColor.Add(TraceEventType.Verbose, ConsoleColor.DarkGray);

            this.eventColor.Add(TraceEventType.Information, ConsoleColor.Gray);

            this.eventColor.Add(TraceEventType.Warning, ConsoleColor.Yellow);

            this.eventColor.Add(TraceEventType.Error, ConsoleColor.Magenta);

            this.eventColor.Add(TraceEventType.Critical, ConsoleColor.Red);

            this.eventColor.Add(TraceEventType.Start, ConsoleColor.DarkCyan);

            this.eventColor.Add(TraceEventType.Stop, ConsoleColor.DarkCyan);
        }

        public ColorConsoleTraceListener(string name) : base(name)
        {
            this.InitializeColors();
        }

        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            this.TraceEvent(eventCache, source, eventType, id, "{0}", message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = this.getEventColor(eventType, originalColor);

            //base.TraceEvent(eventCache, source, eventType, id, format, args);
            if(args == null) // R# is wrong this doesn't always evaluate to false
                this.WriteLine(string.Format(format));
            else
                this.WriteLine(string.Format(format, args));

            Console.ForegroundColor = originalColor;
        }

        ConsoleColor getEventColor(TraceEventType eventType, ConsoleColor defaultColor)
        {
            if (!this.eventColor.ContainsKey(eventType))
            {
                return defaultColor;
            }

            return this.eventColor[eventType];
        }
    }
}
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ABus
{
    public class ABusTraceSource
    {
        TraceSource TraceSource = new TraceSource("ABus");

        public void Information(string message)
        {
            this.TraceSource.TraceInformation(message);
        }

        public void Error(string message)
        {
            this.TraceSource.TraceEvent(TraceEventType.Error, 0, message);
        }

        public void Warning(string message) 
        {
            this.TraceSource.TraceEvent(TraceEventType.Warning, 0, message);
        }

        public void Verbose(string message)
        {
            this.TraceSource.TraceEvent(TraceEventType.Verbose, 0, message);
        }

        public void Critical(string message)
        {
            this.TraceSource.TraceEvent(TraceEventType.Critical, 0, message);
        }

        public void Start(string message)
        {
            this.TraceSource.TraceEvent(TraceEventType.Start, 0, message);
        }

        public void Stop(string message)
        {
            this.TraceSource.TraceEvent(TraceEventType.Stop, 0, message);
        }
    }
}

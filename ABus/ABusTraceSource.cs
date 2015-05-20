using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus
{
    public class ABusTraceSource : TraceSource
    {
        public ABusTraceSource(string name) : base(name)
        {
        }

        public ABusTraceSource(string name, SourceLevels defaultLevel) : base(name, defaultLevel)
        {
        }

        public void TraceInformation(string message)
        {
            this.TraceEvent(TraceEventType.Information, 0, message);
        }

        public void TraceError(string message)
        {
            this.TraceEvent(TraceEventType.Error, 0, message);
        }

        public void TraceWarning(string message)
        {
            this.TraceEvent(TraceEventType.Warning, 0, message);
        }
    }
}

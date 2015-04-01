using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABus.Contracts
{
    public class RawMessage
    {
        public RawMessage()
        {
            this.MetaData = new Dictionary<string, MetaData>();
        }

        /// <summary>
        /// Gets/sets other applicative out-of-band information.
        /// 
        /// </summary>
        public Dictionary<string, MetaData> MetaData { get; set; }


        /// <summary>
        /// Gets/sets the maximum time limit in which the message  must be received.
        /// </summary>
        public TimeSpan TimeToBeReceived { get; set; }

        public string MessageId { get { return null; } }


        /// <summary>
        /// Gets/sets the Id that is used to track messages within a process flow
        /// </summary>
        public string CorrelationId { get { return null; } }

        /// <summary>
        /// Gets/sets the body content of the message
        /// </summary>
        public object Body { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABus.Sample.Contracts.Payments;

namespace ABus.Sample
{
    public class ConfigureHandlerSubscriptionNameTask: IPipelineStartupTask
    {
        /// <summary>
        /// Due to an external party defining the name of the subscription for a queue
        /// its necessary to override the default subscription name for some handlers
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        public void Invoke(PipelineContext context, Action next)
        {
            //// Locate the handler to change from context by its key
            //var key = string.Format("{0}.{1}", "SampleMessageHandler", typeof(MakePaymentCommand).Name);
            //var handler = context.RegisteredHandlers[key];

            //handler.SubscriptionName = "MyCustomSubscriptionName";

            next();
        }
    }
}

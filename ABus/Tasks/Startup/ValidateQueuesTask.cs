using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    public class ValidateQueuesTask : IPipelineStartupTask
    {
        public async Task InvokeAsync(PipelineContext context, Func<Task> next)
        {
            var commonTransportQueuesCreated = new Dictionary<string, string>();

            foreach (var m in context.RegisteredMessageTypes)
            {
                // Check that the message type isn't a response aka Reply as they are special messages handled by a ReplyTo Queue
                if (m.Queue.EndsWith("Response"))
                    continue;

                // get message transport
                var transport = context.TransportInstances[m.Transport.Name];
                var endpoint = new QueueEndpoint {Host = m.Transport.Uri, Name = m.Queue};
                var exists = await transport.QueueExistsAsync(endpoint).ConfigureAwait(false);
                if (exists)
                    context.Trace.Verbose(string.Format("Found: queue: {0}:{1}", m.Transport.Name, m.Queue));
                else
                {
                    context.Trace.Critical(string.Format("NOT FOUND: queue: {0}:{1}", m.Transport.Name, m.Queue));
                    if (context.Configuration.EnsureQueuesExist)
                    {
                        // Check if Auditing is enable and if so ensure the auditing queue is available for this transport
                        if (m.Transport.EnableAuditing)
                        {
                            // Define the audit queue for this transport
                            var auditEndpoint = new QueueEndpoint { Host = m.Transport.Uri, Name = m.Transport.AuditQueue };

                            // Check that we've not already created this queue
                            if (!commonTransportQueuesCreated.ContainsKey(auditEndpoint.ToString()))
                            {
                                if (!await transport.QueueExistsAsync(auditEndpoint).ConfigureAwait(false))
                                {
                                    await transport.CreateQueueAsync(auditEndpoint);

                                    // Now need to add a default subscription so that messages are not lost!
                                    context.Trace.Information(string.Format("Created: audit queue: {0}:{1}", m.Transport.Name, auditEndpoint.Name));
                                }
                            }
                            else
                                commonTransportQueuesCreated.Add(auditEndpoint.ToString(), "");
                        }

                        // Define the error queue for this transport
                        var errorEndpoint = new QueueEndpoint { Host = m.Transport.Uri, Name = m.Transport.ErrorQueue};

                        // Check that we've not already created this queue
                        if (!commonTransportQueuesCreated.ContainsKey(errorEndpoint.ToString()))
                        {
                            if (!await transport.QueueExistsAsync(errorEndpoint).ConfigureAwait(false))
                            {
                                await transport.CreateQueueAsync(errorEndpoint);

                                // Now need to add a default subscription so that messages are not lost!
                                context.Trace.Information(string.Format("Created: error queue: {0}:{1}", m.Transport.Name, errorEndpoint.Name));
                            }
                        }
                        else
                            commonTransportQueuesCreated.Add(errorEndpoint.ToString(), "");

                        // Create queue
                        await transport.CreateQueueAsync(endpoint).ConfigureAwait(false);
                        context.Trace.Information(string.Format("Created: queue: {0}:{1}", m.Transport.Name, m.Queue));
                    }
                }
            }

            // If a reply queue has been defined then validate it too exists
            if (context.Configuration.ReplyToQueue != null)
            {
                var replyQueue = context.Configuration.ReplyToQueue;
                var transportDef = context.Configuration.AvailableTransports[replyQueue.TransportName];
                var transport = context.TransportInstances[replyQueue.TransportName];
                var endpoint = new QueueEndpoint {Host = transportDef.Uri, Name = replyQueue.Endpoint};
                var exists = await transport.QueueExistsAsync(endpoint).ConfigureAwait(false);

                if (exists)
                    context.Trace.Verbose(string.Format("Found: replyTo queue: {0}:{1}", replyQueue.TransportName, endpoint.Name));
                else
                {
                    context.Trace.Critical(string.Format("NOT FOUND: replyTo queue: {0}:{1}", replyQueue.TransportName, endpoint.Name));
                    if (context.Configuration.EnsureQueuesExist)
                    {
                        await transport.CreateQueueAsync(endpoint).ConfigureAwait(false);
                        context.Trace.Information(string.Format("Created: replyTo queue: {0}:{1}", replyQueue.TransportName, endpoint.Name));

                        // A reply queue is a special queue that has two default subscriptions setup
                        // 1. To handle respones that can be handled by any client instance
                        // 2. To handle respones specific for this client instance

                        // 1. Subscription
                        await transport.SubscribeAsync(endpoint, "any").ConfigureAwait(false);

                        // 2. Specific responses
                        await transport.SubscribeAsync(endpoint, Pipeline.ThisEndpointName).ConfigureAwait(false);
                    }
                }
            }
 
            await next().ConfigureAwait(false); 
        }
    } 
}
 
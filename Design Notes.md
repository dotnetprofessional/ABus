# Request Response

There are two categories of request response

1. Response can be handled by any client handler
1. Response must be handled by a specific client handler

## Option 1
A general subscription is created on the ReplyTo Queue which any client instance can handle.
This option is useful when the system just needs to schedule work for itself and know when its
done. It doesn't have to relay the response back to a caller immediately.

## Option 2
A specific subscription is setup for the client instance (ie using process Id) which is then
used as the ReplyTo queue name. As an example:

Queue: MyClientResponses
Subscription: 1234
ReplyTo: MyClientResponses/1234



# Send message to specific subscriber
__Problem:__

Some scenarios require that a mesage be sent to a specific subscriber. 
However, subscribers can only receive messages via the main queue (Azure Service Bus - other
transports may not have this restriction). Therefore need a way to mark a message as being for
a particular subscriber and have all other subscribers ignore the message.

__Solution:__

A naming convention is to be used which can be used by transports that dont support sending 
directly to a subscriber and for those that do. For transports that support direct message
sending to a queue the queue should simply use the naming convention below to name sub-queues.

[queue name]/[subscriber]

For transports that do not support direct sending of a message to sub-queue the transport will
be responsible for parsing the full queue name and adding an additional meta-data element on the
outgoing message.

Meta-data: ABus.ForSubscriber - [subscriber]

This meta-data element is only required by transports that can't send directly to a sub-queue.
This meta-data can then be used by the transport to route the message to the appropriate subscriber.

Example: Azure Service Bus
Send message to subscriber 123 of Topic SampleQueue

Queue: SampleQueue/123

Topic: SampleQueue
Meta-Data: ABus.ForSubscriber - 123

Subscriber needs to be setup for this pattern:

WHERE ForSubscriber = 123 OR ForSubscriber = null

A client process will have two subscriptions to handle the two options:
1. A subscription that handles all non-specific replies
    WHERE ForSubscriber = null

1. A subscription that handles process specific replies
    WHERE ForSubscriber = 123

Also when a request is sent for a specific reply, the TTL should also be set to a default
of 5 minutes. However, this can be overriden on the method. This TTL should also be set
by the receiver sending the reply. This allows for messages that take too long to just expire
the expired messages will be dead-lettered for later analysis.



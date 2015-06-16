using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABus.MemoryServiceBus.ServiceBus
{
    class MemoryQueue
    {
        Action<BrokeredMessage> OnMessageReceived;
        public MemoryQueue(Action<BrokeredMessage> onMessageReceived)
        {
            Queue = new BlockingCollection<BrokeredMessage>();
            OnMessageReceived = onMessageReceived;
            QueueTask = new Task(() => Loop(), TaskCreationOptions.LongRunning);
            QueueTask.Start();
        }

        public MemoryQueue(Func<BrokeredMessage,Task> onMessageReceived, CancellationToken token)
        {
            Queue = new BlockingCollection<BrokeredMessage>();
            QueueTask = new Task(() => Loop(token), TaskCreationOptions.LongRunning);
            QueueTask.Start();
        }

        BlockingCollection<BrokeredMessage> Queue;
        Task QueueTask;

        public void Send(BrokeredMessage message)
        {
            Queue.Add(message);
        }

        public void Publish(BrokeredMessage message)
        {
            Queue.Add(message);
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        void Loop()
        {
            var enumerator = Queue.GetConsumingEnumerable().GetEnumerator();
            DoLoop(enumerator);
        }

        void Loop(CancellationToken cancelToken)
        {
            var enumerator = Queue.GetConsumingEnumerable(cancelToken).GetEnumerator();
            DoLoop(enumerator);
        }

        void DoLoop(IEnumerator<BrokeredMessage> enumerator)
        {
            bool addingCompleted;
            do
            {
                try
                {
                    addingCompleted = !enumerator.MoveNext();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                if (addingCompleted)
                    break;

                Task.Run(() => OnMessageReceived(enumerator.Current)).ConfigureAwait(false);
            }
            while (!Queue.IsCompleted);
        }

        public void Acquire(BrokeredMessage message)
        {
        }

        public void Abandon(BrokeredMessage message)
        {
            Send(message);
        }

        public void Complete(BrokeredMessage message)
        {
            // do nothing
        }
    }
}

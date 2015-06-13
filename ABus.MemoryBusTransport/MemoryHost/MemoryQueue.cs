using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABus.MemoryBusTransport.MemoryHost
{
    class MemoryQueue
    {
        public MemoryQueue()
        {
            QueueTask = new Task(() => Loop(), TaskCreationOptions.LongRunning);
        }

        public MemoryQueue(CancellationToken token)
        {
            QueueTask = new Task(() => Loop(token), TaskCreationOptions.LongRunning);
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

        public void OnMessageAync(Func<BrokeredMessage, Task> onMessage)
        {
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

                DistributeMessage(enumerator.Current);
            }
            while (!Queue.IsCompleted);
        }

        void DistributeMessage(BrokeredMessage rawMessage)
        {
            throw new NotImplementedException();
        }

        public event EventHandler MessageReceived;

    }
}

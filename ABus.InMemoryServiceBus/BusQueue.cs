using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ABus.Contracts;
using System.Threading;

namespace ABus.InMemoryServiceBus
{
    class BusQueue<TMessage>
    {
        public BusQueue()
        {
            TheTask = new Task(() => Loop(), TaskCreationOptions.LongRunning);
        }

        public BusQueue(CancellationToken token)
        {
            TheTask = new Task(() => Loop(token), TaskCreationOptions.LongRunning);
        }

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

        void DoLoop(IEnumerator<TMessage> enumerator)
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
            while (Queue.IsCompleted);
        }

        void DistributeMessage(TMessage rawMessage)
        {
            throw new NotImplementedException();
        }

        BlockingCollection<TMessage> Queue;
        Task TheTask;

    }
}

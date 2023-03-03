using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class Channel<T>
    {
        private readonly SemaphoreSlim _semaphore;

        private readonly ConcurrentQueue<T> _queue;

        private readonly CancellationTokenSource _cancellation;

        public bool isCancelled => _cancellation.IsCancellationRequested;

        public Channel()
        {
            _semaphore = new SemaphoreSlim(0);
            _queue = new ConcurrentQueue<T>();
            _cancellation = new CancellationTokenSource();
        }

        public void Send(T item)
        {
            if (isCancelled) return;

            _queue.Enqueue(item);
            _semaphore.Release();
        }

        public async Task<T> Receive()
        {
            await _semaphore.WaitAsync(_cancellation.Token);

            T item = default;
            // we need a loop here to prevent a race condition that if multiple threads try to 
            // dequeue at the same time, only one of them are going to success, and the other 
            // threads have to retry dequeuing. this is due to a nature of TryDequeue, see below for details:
            // https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1.trydequeue?view=net-6.0#system-collections-concurrent-concurrentqueue-1-trydequeue(-0@)
            while (!isCancelled && !_queue.TryDequeue(out item)) { }

            return item;
        }

        public void Cancel()
        {
            _cancellation.Cancel();
        }
    }
}

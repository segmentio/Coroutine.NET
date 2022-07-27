using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class Dispatcher: IDispatcher
    {
        private readonly TaskFactory _factory;

        public Dispatcher(TaskScheduler scheduler)
        {
            _factory = new TaskFactory(scheduler);
        }

        public Task Post(Action<object> action, SynchronizationContext context)
        {
            return _factory.StartNew(action, context);
        }

        public async Task Send(Func<object, Task> func, SynchronizationContext context)
        {
            await _factory.StartNew(func, context);
        }

        public async Task<T> Async<T>(Func<object, T> func, SynchronizationContext context)
        {
            var result = await _factory.StartNew(func, context);
            return result;
        }
    }
}
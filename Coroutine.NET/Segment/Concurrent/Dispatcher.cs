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

        public Task Post(Action<object> action, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            return _factory.StartNew(action, context)
                .ContinueWith(o =>
                {
                    if (!o.IsFaulted || o.Exception == null) return;
                    handler?.OnExceptionThrown(o.Exception);
                });
        }

        public async Task Send(Func<object, Task> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            await _factory.StartNew(func, context).Unwrap()
                .ContinueWith(o =>
                {
                    if (!o.IsFaulted || o.Exception == null) return;
                    handler?.OnExceptionThrown(o.Exception);
                });
        }

        public async Task<T> Async<T>(Func<object, T> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            var result = await _factory.StartNew(func, context).ContinueWith(o =>
            {
                if (!o.IsFaulted || o.Exception == null) return o.Result;
                
                handler?.OnExceptionThrown(o.Exception);
                return default;

            });
            return result;
        }
    }
}
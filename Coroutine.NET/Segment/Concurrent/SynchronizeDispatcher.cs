using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class SynchronizeDispatcher : IDispatcher
    {
        private readonly TaskScheduler _scheduler;

        public SynchronizeDispatcher()
        {
            _scheduler = new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount);
        }

        public Task Post(Action<object> action, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            var task = new Task(action, context);
            task.RunSynchronously(_scheduler);
            return task;
        }

        public Task Send(Func<object, Task> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            var task = new Task(_ => { func(context); }, context);
            task.RunSynchronously();
            return task;
        }

        public Task<T> Async<T>(Func<object, T> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default)
        {
            var task = new Task<T>(func, context);
            task.RunSynchronously();
            return task;
        }
    }
}

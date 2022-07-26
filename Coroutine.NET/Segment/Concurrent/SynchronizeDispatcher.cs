using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class SynchronizeDispatcher : Dispatcher
    {
        public SynchronizeDispatcher(TaskScheduler scheduler) : base(scheduler)
        {
        }

        public override Task Post(Action<object> action, SynchronizationContext context)
        {
            var task = base.Post(action, context);
            task.RunSynchronously();
            return task;
        }

        public override Task Send(Func<object, Task> func, SynchronizationContext context)
        {
            var task = base.Send(func, context);
            task.RunSynchronously();
            return task;
        }

        public override Task<T> Async<T>(Func<object, T> func, SynchronizationContext context)
        {
            var task = base.Async(func, context);
            task.RunSynchronously();
            return task;
        }
    }
}
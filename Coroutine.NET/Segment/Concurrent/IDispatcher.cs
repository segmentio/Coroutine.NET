using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public interface IDispatcher
    {
        Task Post(Action<object> action, SynchronizationContext context);

        Task Send(Func<object, Task> func, SynchronizationContext context);

        Task<T> Async<T>(Func<object, T> func, SynchronizationContext context);
    }
}
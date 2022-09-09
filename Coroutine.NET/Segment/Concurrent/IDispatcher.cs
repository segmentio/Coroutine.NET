using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public interface IDispatcher
    {
        Task Post(Action<object> action, SynchronizationContext context, ICoroutineExceptionHandler handler = default);

        Task Send(Func<object, Task> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default);

        Task<T> Async<T>(Func<object, T> func, SynchronizationContext context, ICoroutineExceptionHandler handler = default);
    }
}
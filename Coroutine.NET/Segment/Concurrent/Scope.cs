using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class Scope
    {
        private readonly SynchronizationContext _context;

        private readonly ICoroutineExceptionHandler _exceptionHandler;

        public Scope(ICoroutineExceptionHandler exceptionHandler = default)
        {
            _context = exceptionHandler == null ?
                new SynchronizationContext() :
                new SupervisedSynchronizationContext(exceptionHandler);
            _exceptionHandler = exceptionHandler;
        }

        public Scope(SynchronizationContext context)
        {
            _context = context;

            if (context is SupervisedSynchronizationContext supervisedSynchronizationContext)
            {
                _exceptionHandler = supervisedSynchronizationContext._exceptionHandler;
            }
        }

        public void Launch(Func<Task> block)
        {
            _context.Post(state => { block(); }, null);
        }

        public Task Launch(IDispatcher dispatcher, Func<Task> block)
        {
            return dispatcher.Post(_ =>
            {
                SynchronizationContext previous = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(_context);

                block();

                SynchronizationContext.SetSynchronizationContext(previous);
            }, _context, _exceptionHandler);
        }

        public Task Launch(IDispatcher dispatcher, Action block)
        {
            return dispatcher.Post(_ =>
            {
                SynchronizationContext previous = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(_context);

                block();

                SynchronizationContext.SetSynchronizationContext(previous);
            }, _context, _exceptionHandler);
        }

        public async Task<T> Async<T>(IDispatcher dispatcher, Func<T> block)
        {
            T result = await dispatcher.Async(_ =>
            {
                SynchronizationContext previous = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(_context);

                T r = block();

                SynchronizationContext.SetSynchronizationContext(previous);
                return r;
            }, _context, _exceptionHandler);
            return result;
        }

        public static async Task WithContext(IDispatcher dispatcher, Func<Task> block)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ICoroutineExceptionHandler exceptionHandler = default;
            if (context is SupervisedSynchronizationContext supervisedSynchronizationContext)
            {
                exceptionHandler = supervisedSynchronizationContext._exceptionHandler;
            }

            await dispatcher.Send(async (_) =>
            {
                await block();
            }, context, exceptionHandler);
        }

        public static async Task WithContext(IDispatcher dispatcher, Action block)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ICoroutineExceptionHandler exceptionHandler = default;
            if (context is SupervisedSynchronizationContext supervisedSynchronizationContext)
            {
                exceptionHandler = supervisedSynchronizationContext._exceptionHandler;
            }
            await dispatcher.Post(_ =>
            {
                block();
            }, context, exceptionHandler);
        }
    }
}

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
                _exceptionHandler = supervisedSynchronizationContext.ExceptionHandler;
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
                block();
            }, _context, _exceptionHandler);
        }

        public Task Launch(IDispatcher dispatcher, Action block)
        {
            return dispatcher.Post(_ =>
            {
                block();
            }, _context, _exceptionHandler);
        }

        public async Task<T> Async<T>(IDispatcher dispatcher, Func<T> block)
        {
            var result = await dispatcher.Async(_ => block(), _context, _exceptionHandler);
            return result;
        }

        public static async Task WithContext(IDispatcher dispatcher, Func<Task> block)
        {
            var context = SynchronizationContext.Current;
            ICoroutineExceptionHandler exceptionHandler = default;
            if (context is SupervisedSynchronizationContext supervisedSynchronizationContext)
            {
                exceptionHandler = supervisedSynchronizationContext.ExceptionHandler;
            }
            
            await dispatcher.Send(async (_) =>
            {
                await block();
            }, context, exceptionHandler);
        }

        public static async Task WithContext(IDispatcher dispatcher, Action block)
        {
            var context = SynchronizationContext.Current;
            await dispatcher.Post( _ =>
            {
                block();
            }, context);
        }
    }
}
using System;
using System.Threading;

namespace Segment.Concurrent
{
    public class SupervisedSynchronizationContext: SynchronizationContext
    {
        internal readonly ICoroutineExceptionHandler ExceptionHandler;

        public SupervisedSynchronizationContext(ICoroutineExceptionHandler exceptionHandler = default)
        {
            ExceptionHandler = exceptionHandler;
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            try
            {
                d(state);
            }
            catch (Exception e)
            {
                ExceptionHandler?.OnExceptionThrown(e);
            }
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            try
            {
                d(state);
            }
            catch (Exception e)
            {
                ExceptionHandler?.OnExceptionThrown(e);
            }
        }
    }

    public interface ICoroutineExceptionHandler
    {
        void OnExceptionThrown(Exception e);
    }

}
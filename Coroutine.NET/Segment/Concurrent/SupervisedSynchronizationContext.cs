using System;
using System.Threading;

namespace Segment.Concurrent
{
    public class SupervisedSynchronizationContext : SynchronizationContext
    {
        internal readonly ICoroutineExceptionHandler _exceptionHandler;

        public SupervisedSynchronizationContext(ICoroutineExceptionHandler exceptionHandler = default)
        {
            _exceptionHandler = exceptionHandler;
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            SynchronizationContext previous = Current;
            SetSynchronizationContext(this);

            try
            {
                d(state);
            }
            catch (Exception e)
            {
                _exceptionHandler?.OnExceptionThrown(e);
            }

            SetSynchronizationContext(previous);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            SynchronizationContext previous = Current;
            SetSynchronizationContext(this);

            try
            {
                d(state);
            }
            catch (Exception e)
            {
                _exceptionHandler?.OnExceptionThrown(e);
            }

            SetSynchronizationContext(previous);
        }
    }

    public interface ICoroutineExceptionHandler
    {
        void OnExceptionThrown(Exception e);
    }

}

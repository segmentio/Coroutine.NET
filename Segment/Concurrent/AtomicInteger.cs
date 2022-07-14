using System.Threading;

namespace Segment.Concurrent
{
    public class AtomicInteger
    {
        private int _value;

        public AtomicInteger(int value)
        {
            _value = value;
        }

        public int IncrementAndGet()
        {
            Interlocked.Increment(ref _value);
            return _value;
        }

        public void Set(int newValue)
        {
            Interlocked.Exchange(ref _value, newValue);
        }
    }

    public class AtomicBool
    {
        private long _value;

        public AtomicBool(bool value)
        {
            _value = value ? 1 : 0;
        }

        public bool Get()
        {
            _value = Interlocked.Read(ref _value);
            return _value == 1;
        }

        public void Set(bool value)
        {
            Interlocked.Exchange(ref _value, value ? 1 : 0);
        }
    }
}

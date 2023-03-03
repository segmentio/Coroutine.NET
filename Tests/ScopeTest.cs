using System.Threading.Tasks;
using Segment.Concurrent;
using Xunit;

namespace Tests
{
    public class ScopeTest
    {
        private readonly Scope _scope = new Scope();

        private readonly IDispatcher _dispatcher = new Dispatcher(new LimitedConcurrencyLevelTaskScheduler(1));

        private readonly IDispatcher _syncDispatcher = new SynchronizeDispatcher();

        [Fact]
        public async Task TestLaunchAsync()
        {
            bool called = false;
            await _scope.Launch(_dispatcher, () =>
            {
                called = true;
            });

            Assert.True(called);
        }

        [Fact]
        public void TestLaunch()
        {
            bool called = false;
            _scope.Launch(_syncDispatcher, () =>
            {
                called = true;
            });

            Assert.True(called);
        }

        [Fact]
        public async Task TestLaunchWithNoDispatcher()
        {
            bool called = false;
            _scope.Launch(() => {
                called = true;
                return Task.CompletedTask;
            });

            await Task.Delay(500);
            Assert.True(called);
        }

        [Fact]
        public async Task TestAsyncAsync()
        {
            bool called = false;
            await _scope.Launch(_dispatcher, async () =>
            {
                called = await _scope.Async(_syncDispatcher, () => true);
            });

            Assert.True(called);
        }

        [Fact]
        public void TestAsync()
        {
            bool called = false;
            _scope.Launch(_syncDispatcher, async () =>
            {
                called = await _scope.Async(_syncDispatcher, () => true);
            });

            Assert.True(called);
        }

        [Fact]
        public async Task TestWithContextAsync()
        {
            bool called = false;

            await Scope.WithContext(_dispatcher, async () =>
            {
                await _scope.Launch(_dispatcher, () =>
                {
                    called = true;
                });
            });

            Assert.True(called);
        }

        [Fact]
        public void TestWithContext()
        {
            bool called = false;
            _scope.Launch(_syncDispatcher, async () =>
            {
                await Scope.WithContext(_syncDispatcher, () =>
                {
                    called = true;
                });
            });

            Assert.True(called);
        }
    }
}

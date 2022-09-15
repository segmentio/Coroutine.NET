using System.Threading.Tasks;
using Segment.Concurrent;
using Xunit;

namespace Tests
{
    public class ScopeTest
    {
        private Scope scope = new Scope();
        
        private IDispatcher dispatcher = new Dispatcher(new LimitedConcurrencyLevelTaskScheduler(1));
        
        private IDispatcher syncDispatcher = new SynchronizeDispatcher();
        
        [Fact]
        public async Task TestLaunchAsync()
        {
            var called = false;
            await scope.Launch(dispatcher, () =>
            {
                called = true;
            });
            
            Assert.True(called);
        }
        
        [Fact]
        public void TestLaunch()
        {
            var called = false;
            scope.Launch(syncDispatcher, () =>
            {
                called = true;
            });
            
            Assert.True(called);
        }
        
        [Fact]
        public async Task TestLaunchWithNoDispatcher()
        {
            var called = false;
            scope.Launch(async () =>
            {
                called = true;
            });

            await Task.Delay(500);
            Assert.True(called);
        }
        
        [Fact]
        public async Task TestAsyncAsync()
        {
            var called = false;
            await scope.Launch(dispatcher, async () =>
            {
                called = await scope.Async(syncDispatcher, () => true);
            });
            
            Assert.True(called);
        }
        
        [Fact]
        public void TestAsync()
        {
            var called = false;
            scope.Launch(syncDispatcher, async () =>
            {
                called = await scope.Async(syncDispatcher, () => true);
            });
            
            Assert.True(called);
        }

        [Fact]
        public async Task TestWithContextAsync()
        {   
            var called = false;
            
            await Scope.WithContext(dispatcher, async () =>
            {
                await scope.Launch(dispatcher, () =>
                {
                    called = true;
                });
            });
            
            Assert.True(called);
        }
        
        [Fact]
        public void TestWithContext()
        {   
            var called = false;
            scope.Launch(syncDispatcher, async () =>
            {
                await Scope.WithContext(syncDispatcher, () =>
                {
                    called = true;
                });
            });
            
            Assert.True(called);
        }
    }
}
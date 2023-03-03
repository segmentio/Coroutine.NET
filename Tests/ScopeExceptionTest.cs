using System;
using System.Threading.Tasks;
using Segment.Concurrent;
using Xunit;

namespace Tests
{
    public class ScopeExceptionTest
    {
        private readonly ExceptionHandler handler;

        private readonly Scope scope;

        private readonly IDispatcher dispatcher;

        public ScopeExceptionTest()
        {
            handler = new ExceptionHandler();
            scope = new Scope(handler);
            dispatcher = new Dispatcher(new LimitedConcurrencyLevelTaskScheduler(1));
        }

        [Fact]
        public async Task TestLaunchAsync()
        {
            await scope.Launch(dispatcher, () => throw new Exception());
            Assert.True(handler.ExcpetionCaught);
        }

        [Fact]
        public async Task TestLaunch()
        {
            scope.Launch(dispatcher, () => throw new Exception());
            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        }

        [Fact]
        public async Task TestLaunchWithNoDispatcher()
        {
            scope.Launch(() => throw new Exception());
            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        }

        [Fact]
        public async Task TestAsyncAsync()
        {
            bool called = false;
            await scope.Launch(dispatcher, async () =>
            {
                called = await scope.Async<bool>(dispatcher, () => throw new Exception());
            });
            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        }

        [Fact]
        public async Task TestAsync()
        {
            bool called = false;
            scope.Launch(dispatcher, async () =>
            {
                called = await scope.Async<bool>(dispatcher, () => throw new Exception());
            });
            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        }

        [Fact]
        public void TestWithContextAsync() => scope.Launch(async () =>
        {
            await Scope.WithContext(dispatcher,
                async () => { await scope.Launch(dispatcher, () => throw new Exception()); });

            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        });

        [Fact]
        public void TestWithContext() => scope.Launch(async () =>
        {
            scope.Launch(dispatcher, async () =>
            {
                await Scope.WithContext(dispatcher, () => throw new Exception());
            });

            await Task.Delay(500);
            Assert.True(handler.ExcpetionCaught);
        });
    }

    class ExceptionHandler : ICoroutineExceptionHandler
    {
        public bool ExcpetionCaught = false;

        public void OnExceptionThrown(Exception e)
        {
            ExcpetionCaught = true;
        }
    }
}

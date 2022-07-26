﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Segment.Concurrent
{
    public class Scope
    {
        private readonly SynchronizationContext _context;

        public Scope()
        {
            _context = new SynchronizationContext();
        }

        public Scope(SynchronizationContext context)
        {
            _context = context;
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
            }, _context);
        }

        public Task Launch(IDispatcher dispatcher, Action block)
        {
            return dispatcher.Post(_ =>
            {
                block();
            }, _context);
        }

        public async Task<T> Async<T>(IDispatcher dispatcher, Func<T> block)
        {
            var result = await dispatcher.Async(_ => block(), _context);
            return result;
        }

        public static async Task WithContext(IDispatcher dispatcher, Func<Task> block)
        {
            var context = SynchronizationContext.Current;
            await dispatcher.Send(async (_) =>
            {
                await block();
            }, context);
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
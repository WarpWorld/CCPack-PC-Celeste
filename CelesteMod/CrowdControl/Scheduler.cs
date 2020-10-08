using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CrowdControl;
using CrowdControl.Common;
using JetBrains.Annotations;
using Log = CrowdControl.Common.Log;

namespace CrowdControl.Client
{
    public class Scheduler : IDisposable
    {
        private const int MAX_ATTEMPTS = 24;

        private readonly Random _rng = new Random();

        private readonly Action<EffectRequest> _callback;

        private readonly ConcurrentQueue<SchedulerEntry> _queue = new ConcurrentQueue<SchedulerEntry>();
        private readonly ConcurrentDictionary<Guid, SchedulerEntry> _context = new ConcurrentDictionary<Guid, SchedulerEntry>();

        private readonly CancellationTokenSource _quitting = new CancellationTokenSource();

        private readonly Task _worker;

        ~Scheduler() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try { _quitting.Cancel(); }
            catch { /**/ }
        }

        public Scheduler([NotNull] Action<EffectRequest> callback)
        {
            _callback = callback;
            _worker = Task.Factory.StartNew(ProcessMessages, TaskCreationOptions.LongRunning);
        }

        private class SchedulerEntry
        {
            public int AttemptCount = 0;
            public readonly EffectRequest Request;
            public DateTimeOffset RetryAfter = DateTimeOffset.MinValue;

            public SchedulerEntry(EffectRequest request) => Request = request;
        }

        public void Enqueue(EffectRequest request)
        {
            SchedulerEntry entry = new SchedulerEntry(request);
            _context[request.ID] = entry;
            _queue.Enqueue(entry);
        }

        public async Task ProcessMessages()
        {
            while (!_quitting.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out SchedulerEntry entry))
                {
                    TimeSpan delayRemaining = entry.RetryAfter - DateTimeOffset.UtcNow;
                    if (delayRemaining > TimeSpan.Zero)
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(delayRemaining);
                            _queue.Enqueue(entry);
                        }, _quitting.Token).Forget();
                    }
                    try { _callback(entry.Request); }
                    catch (Exception e) { Log.Error(e); }
                    await Task.Delay(250);
                }
                else { await Task.Delay(1000); }
            }
        }

        [SuppressMessage("ReSharper", "EmptyEmbeddedStatement")]
        public void Flush()
        {
            while (_queue.TryDequeue(out _));
            _context.Clear();
        }

        public void Clear(EffectRequest request) => _context.TryRemove(request.ID, out _);

        [SuppressMessage("ReSharper", "PatternAlwaysMatches")]
        public DateTimeOffset? Delay(EffectRequest request)
        {
            Guid id = request.ID;
            if (_context.TryGetValue(id, out SchedulerEntry entry))
            {
                int attempt = Interlocked.Increment(ref entry.AttemptCount);
                TimeSpan delay;
                switch (attempt)
                {
                    case int n when (n <= 5):
                        delay = TimeSpan.FromSeconds(n);
                        break;
                    case int n when (n <= MAX_ATTEMPTS):
                        delay = TimeSpan.FromSeconds(n + _rng.Next(0, n / 2));
                        break;
                    default:
                        _context.TryRemove(id, out _);
                        return null;
                }
                DateTimeOffset finalTime = DateTimeOffset.UtcNow + delay;
                entry.RetryAfter = finalTime;
                _queue.Enqueue(entry);
                return finalTime;
            }
            return null;
        }
    }
}

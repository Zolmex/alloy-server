#region

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Common.Utilities
{
    public sealed class LockAsync : IDisposable
    {
        private static readonly ConcurrentDictionary<object, SemaphoreSlim> _lockObjs = new();
        private readonly SemaphoreSlim _semaphore;

        public LockAsync(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        // Remember to await the returned task
        public static async Task<LockAsync> Lock(object obj)
        {
            if (!_lockObjs.TryGetValue(obj, out var semaphore))
                _lockObjs[obj] = semaphore = new SemaphoreSlim(1, 1);
            await semaphore.WaitAsync();
            return new LockAsync(semaphore);
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
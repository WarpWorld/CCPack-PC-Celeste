using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CrowdControl
{
    internal static class Extensions
    {
        [DebuggerStepThrough]
        public static async void Forget(this Task task)
        {
            try { await task.ConfigureAwait(false); }
            catch (Exception ex) { Log.Error(ex); }
        }

        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default(CancellationTokenRegistration);
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                tokenRegistration = cancellationToken.Register(
                    state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    tcs);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
        {
            return handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
        }

        public static async Task<IDisposable> UseWaitAsync(this SemaphoreSlim semaphore, CancellationToken cancelToken = default)
        {
            await semaphore.WaitAsync(cancelToken).ConfigureAwait(false);
            return new ReleaseWrapper(semaphore);
        }

        private class ReleaseWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed;

            public ReleaseWrapper(SemaphoreSlim semaphore) => _semaphore = semaphore;

            public void Dispose()
            {
                if (_disposed) { return; }
                _semaphore.Release();
                _disposed = true;
            }
        }

        public static string ToHexString(this byte[] data) => BitConverter.ToString(data).Replace("-", string.Empty);

        public static byte[] ToBytes(this string data)
        {
            if (data.Length % 2 == 1) { throw new Exception("Byte array cannot have an odd number of digits."); }

            byte[] arr = new byte[data.Length >> 1];

            for (int i = 0; i < data.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(data[i << 1]) << 4) + (GetHexVal(data[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}

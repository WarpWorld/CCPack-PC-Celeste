using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CrowdControl
{
    public class SimpleTCPClient : IDisposable
    {
        private TcpClient _client;
        private readonly SemaphoreSlim _client_lock = new SemaphoreSlim(1);
        private readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _error = new ManualResetEventSlim(false);

        public bool Connected { get; private set; }

        private readonly CancellationTokenSource _quitting = new CancellationTokenSource();

        public SimpleTCPClient()
        {
            Task.Factory.StartNew(ConnectLoop, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        ~SimpleTCPClient() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _quitting.Cancel();
            if (disposing) { _client.Close(); }
        }

        private async void ConnectLoop()
        {
            while (!_quitting.IsCancellationRequested)
            {
                try
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync("127.0.0.1", 58430);
                    if (!_client.Connected) { continue; }
                    Connected = true;
                    _ready.Set();
                    await _error.WaitHandle.WaitOneAsync(_quitting.Token);
                }
                catch (Exception e) { Log.Error(e); }
                finally
                {
                    Connected = false;
                    _error.Reset();
                    _ready.Reset();
                    try { _client.Close(); }
                    catch { /**/ }
                    if (!_quitting.IsCancellationRequested) { await Task.Delay(TimeSpan.FromSeconds(1)); }
                }
            }
            Connected = false;
        }

        private async void Listen()
        {
            List<byte> mBytes = new List<byte>();
            byte[] buf = new byte[4096];
            while (!_quitting.IsCancellationRequested)
            {
                try
                {
                    if (!(await _ready.WaitHandle.WaitOneAsync(_quitting.Token))) { continue; }
                    Socket socket = _client.Client;

                    int bytesRead = socket.Receive(buf);

                    foreach (byte b in buf.Take(bytesRead))
                    {
                        if (b != 0) { mBytes.Add(b); }
                        else
                        {
                            string json = Encoding.UTF8.GetString(mBytes.ToArray());
                            Request req = JsonConvert.DeserializeObject<Request>(json);
                            try { RequestReceived?.Invoke(req); }
                            catch (Exception e) { Log.Error(e); }
                            mBytes.Clear();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    _error.Set();
                }
                finally { if (!_quitting.IsCancellationRequested) { await Task.Delay(TimeSpan.FromSeconds(1)); } }
            }
        }

        public event Action<Request> RequestReceived;

        public async Task<bool> Respond(Response response)
        {
            string json = JsonConvert.SerializeObject(response);
            byte[] buffer = Encoding.UTF8.GetBytes(json + '\0');
            Socket socket = _client.Client;
            await _client_lock.WaitAsync();
            try
            {
                int bytesSent = socket.Send(buffer);
                return bytesSent > 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
            finally { _client_lock.Release(); }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum RequestType
        {
            Test = 0,
            Start = 1,
            Stop = 2
        }

        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnassignedField.Local")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class Request
        {
            private static int _next_id = 0;
            public int id = Interlocked.Increment(ref _next_id);
            public string code;
            public object[] parameters;
            public string viewer;
            public RequestType type;
        }

        [Serializable]
        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnassignedField.Local")]
        public class Response
        {
            public int id;
            public EffectResult status;
            public string message;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public enum EffectResult
        {
            /// <summary>The effect executed successfully.</summary>
            Success = 0,
            /// <summary>The effect failed to trigger, but is still available for use. Viewer(s) will be refunded.</summary>
            Failure = 1,
            /// <summary>Same as <see cref="Failure"/> but the effect is no longer available for use.</summary>
            Unavailable = 2,
            /// <summary>The effect cannot be triggered right now, try again in a few seconds.</summary>
            Retry = 3,
            /// <summary>INTERNAL USE ONLY. The effect has been queued for execution after the current one ends.</summary>
            Queue = 4,
            /// <summary>INTERNAL USE ONLY. The effect triggered successfully and is now active until it ends.</summary>
            Running = 5
        }
    }
}

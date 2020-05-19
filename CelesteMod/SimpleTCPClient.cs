using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Celeste.Mod.CrowdControl
{
    public class SimpleTCPClient : IDisposable
    {
        private readonly TcpClient _client = new TcpClient();
        private readonly SemaphoreSlim _client_lock = new SemaphoreSlim(1);
        private readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _error = new ManualResetEventSlim(false);

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

        public void Dispose(bool disposing)
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
                    await _client.ConnectAsync("127.0.0.1", 58430);
                    if (!_client.Connected) { continue; }
                    _ready.Set();
                    await _error.WaitHandle.WaitOneAsync(_quitting.Token);
                }
                catch (Exception e) { Log.Error(e); }
                finally
                {
                    _error.Reset();
                    _ready.Reset();
                    if (!_quitting.IsCancellationRequested) { await Task.Delay(TimeSpan.FromSeconds(1)); }
                }
            }
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

                    /*var result = socket.BeginReceive(buf, 0, buf.Length, SocketFlags.None, _ => { }, null);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    int bytesRead = await Task.Factory.FromAsync(result, r => socket.EndSend(r));*/
                    int bytesRead = socket.Receive(buf);

                    foreach (byte b in buf.Take(bytesRead))
                    {
                        if (b != 0) { mBytes.Add(b); }
                        else
                        {
                            string json = Encoding.UTF8.GetString(mBytes.ToArray());
                            Request req = JsonConvert.DeserializeObject<Request>(json);
                            try { RequestReceived?.Invoke(this, req); }
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

        public event EventHandler<Request> RequestReceived;

        private readonly byte[] RESPONSE_TERMINATOR = new byte[] { 0 };
        public async Task<bool> Respond(Response response)
        {
            string json = JsonConvert.SerializeObject(response);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            Socket socket = _client.Client;
            await _client_lock.WaitAsync();
            try
            {
                /*var result = socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, _ => { }, null);
                // ReSharper disable once AssignNullToNotNullAttribute
                int bytesSent = await Task.Factory.FromAsync(result, r => socket.EndSend(r));*/
                int bytesSent = socket.Send(buffer) + socket.Send(RESPONSE_TERMINATOR);
                return bytesSent > 0;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
            finally { _client_lock.Release(); }
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
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
        public class Request
        {
            public int id;
            public string code;
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

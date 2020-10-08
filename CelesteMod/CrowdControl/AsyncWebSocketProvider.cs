using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CrowdControl;
using Log = CrowdControl.Common.Log;

namespace CrowdControl.Client
{
    public class AsyncWebSocketProvider : IDisposable
    {
        private ClientWebSocket _ws;
        private readonly CancellationTokenSource _quitting = new CancellationTokenSource();
        private readonly SemaphoreSlim _ws_lock = new SemaphoreSlim(1);

        public event Action<string> OnMessageReceived;
        public event Action<Exception, string> OnError;
        public event Action OnDisconnected;
        public event Action OnConnected;

        public bool Connected => _ws?.State == WebSocketState.Open;

        ~AsyncWebSocketProvider() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing) { _quitting.Cancel(); }
        }

        public AsyncWebSocketProvider()
        {
            Task.Factory.StartNew(async () =>
            {
                ArraySegment<byte> buf = new ArraySegment<byte>(new byte[4096]);
                List<byte> msg = new List<byte>();
                while (!_quitting.IsCancellationRequested)
                {
                    try
                    {
                        if (!Connected)
                        {
                            OnDisconnected?.Invoke();
                            if (!_quitting.IsCancellationRequested) { await Task.Delay(TimeSpan.FromSeconds(1)); }
                            continue;
                        }
                        WebSocketReceiveResult r = await _ws.ReceiveAsync(buf, _quitting.Token);
                        //Log.Message($"AsyncWebSocketProvider got a block (E:{r.EndOfMessage}): {Encoding.UTF8.GetString(buf.ToArray())}");
                        msg.AddRange(buf.Take(r.Count));

                        if (r.EndOfMessage)
                        {
                            try { OnMessageReceived?.Invoke(Encoding.UTF8.GetString(msg.ToArray())); }
                            catch (Exception e) { Log.Error(e); }
                            msg.Clear();
                        }
                    }
                    catch (Exception e)
                    {
                        OnError?.Invoke(e, e.Message);
                        if (!_quitting.IsCancellationRequested) { await Task.Delay(TimeSpan.FromSeconds(1)); }
                    }
                }
            }, TaskCreationOptions.LongRunning).Forget();
        }

        public async Task<bool> Connect(string url)
        {
            using (await _ws_lock.UseWaitAsync())
            {
                OnDisconnected?.Invoke();
                _ws = new ClientWebSocket();
                try
                {
                    await _ws.ConnectAsync(new Uri(url), _quitting.Token);
                    //Log.Message("Connected to server socket.");
                    if (!Connected) { return false; }
                    OnConnected?.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    //Log.Message("Failed to connect to server socket.");
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> Exit()
        {
            using (await _ws_lock.UseWaitAsync())
            {
                try
                {
                    var ws = _ws;
                    if (ws != null) { await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _quitting.Token); }
                }
                catch { return false; }
                return true;
            }
        }

        public async Task<bool> Send(string message)
        {
            using (await _ws_lock.UseWaitAsync())
            {
                if (!Connected) { return false; }
                try { await _ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Binary, true, _quitting.Token); }
                catch { return false; }
                return true;
            }
        }
    }
}

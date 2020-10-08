using System;
using System.Threading;
using System.Threading.Tasks;
using Celeste.Mod.CrowdControl;
using CrowdControl.Common;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Log = CrowdControl.Common.Log;

namespace CrowdControl.Client
{
    internal class RemoteServiceClient : IDisposable
    {
        private const string ENDPOINT = "wss://endpoint.crowdcontrol.live";

        private readonly Guid _player_token;

        private readonly string _game_endpoint;

        public event Action<InitializationBlock> OnInitialization;

        public event Action<EffectRequest> OnEffectRequest;
        public event Action<DataRequest> OnDataRequest;
        public event Action<MessageBlock> OnMessage;
        public event Action<ShutdownBlock> OnShutdown;

        [CanBeNull] public InitializationBlock InitBlock { get; private set; }

        [CanBeNull] public Player Identity => InitBlock?.Player;

        [NotNull] public Game Game;

        [CanBeNull] public Channel Channel { get; private set; }

        public DateTimeOffset LastActivity { get; private set; } = DateTimeOffset.MinValue;

        //[NotNull, UsedImplicitly] private readonly DataAwaiter _data_awaiter;


        private readonly AsyncWebSocketProvider _socket = new AsyncWebSocketProvider();

        private int _retries;
        private readonly Timer _retry_timer;

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) { _socket.Dispose(); }
        }

        ~RemoteServiceClient() => Dispose(false);

        public RemoteServiceClient([NotNull] Game game)
        {
            Game = game;
            _socket.OnMessageReceived += OnSocketMessageReceived;
        }

        private void OnSocketMessageReceived(string message)
        {
            try
            {
                switch (CrowdControlBlock.Deserialize(message))
                {
                    case DataRequest dr:
                        OnDataRequest?.Invoke(dr);
                        break;
                    case EffectRequest er:
                        OnEffectRequest?.Invoke(er);
                        return;
                    case InitializationBlock ib:
                        OnInitialization?.Invoke(ib);
                        break;
                    case MessageBlock mb:
                        OnMessage?.Invoke(mb);
                        break;
                    case ShutdownBlock sb:
                        OnShutdown?.Invoke(sb);
                        break;
                    case KeepAlive ka:
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public void Start()
        {
            _socket.Connect(ENDPOINT).Forget();
        }
        public bool SendBlockSync([NotNull] CrowdControlBlock response) => SendBlock(response).Result;

        public async Task<bool> SendBlock([NotNull] CrowdControlBlock response)
        {
            if (!_socket.Connected) { return false; }
            var json = JsonConvert.SerializeObject(response);
            try
            {
                return await _socket.Send(json);
            }
            catch
            {
                return false; // TODO: report these errors
            }
        }

        public async Task<bool> Respond([NotNull] EffectRequest request, EffectStatus status, string message = null)
        {
            return await SendBlock(new EffectResponse(request.ID, status, message ?? string.Empty));
        }

        public async Task<bool> Respond([NotNull] EffectRequest request, EffectStatus status, DateTimeOffset? dueTime, string message = null)
        {
            return await SendBlock(new EffectResponse(request.ID, status, dueTime, message ?? string.Empty));
        }
    }
}

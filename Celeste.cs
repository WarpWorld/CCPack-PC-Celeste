using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectorLib;
using CrowdControl.Common;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs
{
    public class Celeste : EffectPack<NullConnector>
    {
        private const string HOST = "127.0.0.1";
        private const ushort PORT = 61411;

        private readonly TcpListener _server = new TcpListener(IPAddress.Parse(HOST), PORT);
        private readonly ConcurrentDictionary<TcpClient, NetworkStream> _clients = new ConcurrentDictionary<TcpClient, NetworkStream>();
        private volatile bool _quitting = false;
        private readonly Task _listener;
        private readonly ManualResetEventSlim _start_wait = new ManualResetEventSlim(false);
        private readonly ConcurrentDictionary<uint, Action<Response>> _handlers = new ConcurrentDictionary<uint, Action<Response>>();

        private bool HasClients => !_clients.IsEmpty;

        ~Celeste() => Dispose();

        public override void Dispose()
        {
            if (_quitting) { return; }
            base.Dispose();
            _quitting = true;
            try { _server.Stop(); }
            catch { }
            CloseAllSockets();
        }

        private void CloseAllSockets()
        {
            foreach (var c in _clients.Keys)
            {
                try { c.Close(); }
                catch (Exception e) { Log.Error(e); }
                _clients.TryRemove(c, out _);
            }
        }

        public Celeste([NotNull] IPlayer player,
            [NotNull] Func<CrowdControlBlock, bool> responseHandler, [NotNull] Action<object> statusUpdateHandler) :
            base(player, responseHandler, statusUpdateHandler)
        {
            _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listener = Task.Run(Listen);
            OnConnectorAttached += (s, c) =>
            {
                if (c == null)
                {
                    try { _server.Stop(); }
                    catch (Exception e) { Log.Error(e); }
                    CloseAllSockets();
                    _start_wait.Reset();
                }
                else
                {
                    _server.Start();
                    _start_wait.Set();
                }
            };
        }

        private async Task Listen()
        {
            _start_wait.Wait();
            while (!_quitting)
            {
                Log.Debug("Service is listening for incoming connections...");
                TcpClient client = _server.AcceptTcpClient();

                Log.Debug($"Starting message handler for game {client.GetHashCode()}.");
                Task.Run(() =>
                {
                    Log.Debug($"Game {client.GetHashCode()} connected.");
                    try
                    {
                        NetworkStream ns = client.GetStream();
                        _clients[client] = ns;
                        StringBuilder s = new StringBuilder();

                        using (StreamReader r = new StreamReader(ns))
                        {
                            while (client.Connected)
                            {
                                int next;
                                while ((next = r.Read()) != 0) { s.Append((char)next); }
                                if (s.Length > 0)
                                {
                                    Log.Debug($"Got a message from game {client.GetHashCode()}.");
                                    string msg = s.ToString();
                                    //Task.Run(() => MessageReceived(msg)).Forget();
                                    MessageReceived(msg);
                                    s.Clear();
                                }
                            }
                        }
                    }
                    catch { Log.Debug($"Game {client.GetHashCode()}'s socket threw an exception."); }
                    finally
                    {
                        Log.Debug($"Game {client.GetHashCode()} disconnected.");
                        client.Dispose();
                        _clients.TryRemove(client, out _);
                    }
                }).Forget();
            }
        }

        public override Game Game { get; } = new Game(5, "Celeste", "Celeste", "PC", ConnectorType.NullConnector);

        protected override void RequestData(DataRequest request) => Respond(request, request.Key, null, false, $"Variable name \"{request.Key}\" not known");

        public override List<Effect> Effects
        {
            get
            {
                List<Effect> effects = new List<Effect>
                {
                    new Effect("Add Enemy", "AddEnemies"),
                    new Effect("Randomizer", "BabyRandomize"),
                    new Effect("Unidentify", "Unidentify"),
                    new Effect("Identify", "Identify"),
                    new Effect("Skill Boost", "SkillBoost"),
                    new Effect("Mystery Present", "MysteryPresent"),
                    new Effect("Heal", "HealPlayer"),
                    new Effect("Bucks", "Bucks"),
                    new Effect("Good Earthling", "GoodEarthling"),
                    new Effect("Scramble Stats", "UFOStats"),
                    new Effect("Instant Demotion", "Derank"),
                    new Effect("Promotion", "Promote"),
                    new Effect("Hula", "Hula"),
                    //new Effect("HyperFunk Zone", "HFZ"),
                    new Effect("DanceGame", "DanceGame"),
                    new Effect("Good Food", "GoodFood"),
                    new Effect("Bad Food", "BadFood"),
                    //new Effect("Hot tub", "Hottub"),
                    new Effect("Level 0", "LevelZero"),
                    new Effect("Teleport", "Teleport"),
                    new Effect("Togetherness", "Togetherness"),
                    new Effect("Fart", "Fart"),
                    new Effect("Burp", "Burp"),
                    new Effect("Sneeze", "Sneeze"),
                    new Effect("Steal Present", "StealPresent"),
                    new Effect("Mailbox", "Mailbox"),
                    new Effect("Golden Mailbox", "GoldenMailbox"),
                    new Effect("Reroll Hat", "PowerHat"),
                    new Effect("Light Switch", "LightSwitch"),
                    new Effect("Random Controls", "Confuse"),
                    new Effect("Extreme Enemies", "AgroEnemies"),
                    new Effect("Funkzilla", "BigSize"),
                    new Effect("Nuke", "Nuke"),
                    new Effect("Dance Party", "DanceParty"),
                    new Effect("Ready, Aim, Tomatoes", "TomatoTime"),
                    new Effect("Amp Presents", "Amp2"),
                    new Effect("Nap Time", "SleepyTime"),
                    new Effect("Drop Level", "DropLevel"),
                    new Effect("XP Boost", "XPBoost"),
                    new Effect("X-Mas", "XMas"),
                    new Effect("Uncover Map", "UncoverMap"),
                    new Effect("Rocket Skates", "MeanRocketSkates"),
                    new Effect("Burnin Up", "BurningUp"),
                    new Effect("Innertube", "Innertube"),
                    new Effect("Icarus Wings", "IcarusWings"),
                    new Effect("Rain Cloud", "RainCloud"),
                    new Effect("Here I Am", "HereIAm"),
                    new Effect("Expel Presents", "ExpelPresents"),
                    new Effect("Autosearch", "ComeOut"),
                    new Effect("Freeze Presents", "PresentFreeze"),
                    new Effect("Spring Shoes", "SpringShoes"),
                    new Effect("Hitops", "Hitops"),
                    new Effect("Extra Life", "ExtraLife"),
                    new Effect("Total Bummer", "TotalBummer"),
                    new Effect("Ice", "Ice")
                };
                return effects;
            }
        }

        public override List<ItemType> ItemTypes => new List<ItemType>(new[]
        {
            new ItemType("Quantity", "quantity99", ItemType.Subtype.Slider, "{\"min\":1,\"max\":99}"),
            new ItemType("Quantity", "quantity255", ItemType.Subtype.Slider, "{\"min\":1,\"max\":255}")
        });

        private bool SendJSON(Request req)
        {
            if (!HasClients) { return false; }
            bool result = false;
            string pl = JsonConvert.SerializeObject(req);
            byte[] buf = Encoding.UTF8.GetBytes(pl);
            foreach (var client in _clients.Keys)
            {
                try
                {
                    var stream = client.GetStream();
                    if (!stream.CanWrite) { continue; }
                    lock (stream)
                    {
                        stream.Write(buf, 0, buf.Length);
                        stream.WriteByte(0);
                    }
                    result = true;
                    Log.Debug($"Sent message {req.id} successfully to game {client.GetHashCode()}.");
                }
                catch { Log.Debug($"Failed to send message {req.id} to game {client.GetHashCode()}."); }
            }
            return result;
        }

        protected override bool IsReady(EffectRequest request) => HasClients;

        private void MessageReceived(string data)
        {
            Response r = JsonConvert.DeserializeObject<Response>(data);
            if (_handlers.TryRemove(r.id, out var f)) { f(r); }
        }

        protected override void StartEffect(EffectRequest request)
        {
            if (!IsReady(request))
            {
                DelayEffect(request);
                return;
            }
            Request req = new Request
            {
                code = request.InventoryItem.BaseItem.Code,
                viewer = request.DisplayViewer,
                type = RequestType.Start
            };

            _handlers[req.id] = r => { Respond(request, (r.status == EffectResult.Success) ? EffectStatus.Success : EffectStatus.FailTemporary); };
            if (!SendJSON(req))
            {
                _handlers.TryRemove(req.id, out _);
                DelayEffect(request);
            }
        }

        protected override bool StopEffect(EffectRequest request)
        {
            Request r = new Request
            {
                code = request.InventoryItem.BaseItem.Code,
                viewer = request.DisplayViewer,
                type = RequestType.Stop
            };
            return SendJSON(r);
        }

        private enum RequestType
        {
            Test = 0,
            Start = 1,
            Stop = 2
        }

        [Serializable]
        private class Request
        {
            private static uint _next_id = 0;
            public uint id = unchecked(_next_id++);
            public string code;
            public string viewer;
            public RequestType type;
        }

        [Serializable]
        private class Response
        {
            public uint id;
            public EffectResult status;
        }

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

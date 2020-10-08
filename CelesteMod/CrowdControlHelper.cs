using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CrowdControl.Client;
using CrowdControl.Common;
using Microsoft.Xna.Framework;
using Monocle;
using Game = Microsoft.Xna.Framework.Game;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlHelper : DrawableGameComponent
    {
        static CrowdControlHelper()
        {
            global::CrowdControl.Common.Log.OnMessage += Log.Message;
        }

        public static CrowdControlHelper Instance;

        //private readonly SimpleTCPClient _client;
        private readonly RemoteServiceClient _service_client;
        private readonly Scheduler _scheduler;

        private string _gui_message = "";

        public bool GameReady = false;

        public Player Player;

        public static readonly global::CrowdControl.Common.Game CC_GAME = new global::CrowdControl.Common.Game(5, "Celeste", "Celeste", "PC", ConnectorType.NullConnector);

        public readonly Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
        public IEnumerable<Effect> Active => Effects.Select(e => e.Value).Where(e => e.Active);

        public static void Add()
        {
            if (Instance != null) { return; }

            Instance = new CrowdControlHelper(Celeste.Instance);
            Celeste.Instance.Components.Add(Instance);
        }

        public CrowdControlHelper(Game game) : base(game)
        {
            Log.Message("CrowdControl handler is starting...");
            // Update before and draw after the game.
            UpdateOrder = -10000;
            DrawOrder = 10000;

            // Find all actions and collect them.
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (!typeof(Effect).IsAssignableFrom(type) || type.IsAbstract) { continue; }

                Effect action = (Effect)type.GetConstructor(Everest._EmptyTypeArray).Invoke(Everest._EmptyObjectArray);
                action.Load();
                Effects.Add(action.Code, action);

                // For debugging purposes: Add a DebugRC helper.
                /*Everest.DebugRC.EndPoints.Add(new RCEndPoint
                {
                    Path = $"/crowdcontrol/{action.Code}",
                    PathHelp = $"/crowdcontrol/{action.Code}?active={{true|false}} (no value to toggle)",
                    PathExample = $"/crowdcontrol/{action.Code}",
                    Name = $"CrowdControl: {action.Code}",
                    InfoHTML = "CrowdControl action endpoint for testing purposes.",
                    Handle = c => {
                        NameValueCollection data = Everest.DebugRC.ParseQueryString(c.Request.RawUrl);
                        bool active;
                        if (!bool.TryParse(data["active"], out active)){ active = !action.Active;}
                        //lock (Queue)
                        {
                            Queue.Enqueue(Tuple.Create(action, active));
                        }
                        Everest.DebugRC.Write(c, $@"{{""active"": {(active ? "true" : "false")}}}");
                    }
                });*/
            }

            Log.Message("Creating scheduler...");
            _scheduler = new Scheduler(RequestReceived);
            Log.Message("Starting service client...");
            _service_client = new RemoteServiceClient(CC_GAME);
            _service_client.OnInitialization += OnClientInitialized;
            _service_client.OnEffectRequest += _scheduler.Enqueue;
            _service_client.Start();
        }

        private void OnClientInitialized(InitializationBlock obj)
        {
            global::CrowdControl.Common.Log.Message("Client got an initialization block.");
            _service_client.SendBlock(new GameSelection(CC_GAME, _service_client.Channel)).Forget();
        }

        public static void Remove()
        {
            if (Instance == null) { return; }

            //Instance._client.Dispose();
            Instance._service_client.Dispose();

            foreach (Effect action in Instance.Effects.Values)
            {
                try
                {
                    action.TryStop();
                    action.Unload();
                }
                catch (Exception e) { Log.Error(e); }
            }

            //Everest.DebugRC.EndPoints.RemoveAll(ep => ep.Path.StartsWith("/bitraces/"));

            Celeste.Instance.Components.Remove(Instance);
            Instance = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Note: This runs earlier than the game finishes loading!
            if (!(Engine.Scene is GameLoader)) { GameReady = true; }
            if (!GameReady) { return; }

            Player = Engine.Scene?.Tracker?.GetEntity<Player>();

            foreach (Effect action in Active)
            {
                try { action.Update(); }
                catch (Exception e) { Log.Error(e); }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Note: This runs earlier than the game finishes loading!
            if (!GameReady) { return; }

            Monocle.Draw.SpriteBatch.Begin();

            if (!string.IsNullOrWhiteSpace(_gui_message))
            {
                ActiveFont.DrawOutline(_gui_message,
                    Vector2.Zero, Vector2.Zero, Vector2.One * 0.5f,
                    Color.White, 1f, Color.Black);
            }
            /*ActiveFont.DrawOutline(
                $"Hello, World!\nIn-game: {Engine.Scene is Level}",
                Vector2.Zero, // Position in "GUI coordinates" (1920 x 1080)
                Vector2.Zero, // "Center point" inside the text (0f - 1f for both x, y)
                Vector2.One * 0.5f, // Scale (x, y)
                Color.White, // Text color
                1f, // Outline width
                Color.Black // Outline color
            );*/

            foreach (Effect action in Active)
            {
                try { action.Draw(); }
                catch (Exception e) { Log.Error(e); }
            }
            Monocle.Draw.SpriteBatch.End();
        }

        private void RequestReceived(EffectRequest request)
        {
            try
            {
                string code = request.BaseCode;
                Log.Message($"Got an effect request [{request.ID}:{code}].");
                if (!Effects.TryGetValue(code, out Effect effect))
                {
                    Log.Message($"Effect {code} not found.");
                    //could not find the effect
                    _service_client.Respond(request, EffectStatus.FailPermanent).Forget();
                    return;
                }

                if (!effect.TryStart())
                {
                    Log.Message($"Effect {code} could not start.");
                    //could not start the effect
                    DateTimeOffset? delay = _scheduler.Delay(request);
                    _service_client.Respond(request, (delay.HasValue ? EffectStatus.DelayEstimated : EffectStatus.FailTemporary), delay).Forget();
                    return;
                }

                Log.Message($"Effect {code} started.");
                _service_client.Respond(request, EffectStatus.Success).Forget();
                _scheduler.Clear(request);
            }
            catch (Exception e) { Log.Error(e); }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CrowdControl;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlHelper : DrawableGameComponent
    {
        public static CrowdControlHelper Instance;

        private readonly SimpleTCPClient _client;

        private readonly ConcurrentQueue<GUIMessage> _gui_messages = new ConcurrentQueue<GUIMessage>();
        private const int MAX_GUI_MESSAGES = 5;

        private class GUIMessage
        {
            public string message;
            public TimeSpan elapsed;
        }

        public bool GameReady = false;

        private GameTime _last_time = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

        public Player Player;

        public readonly Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
        public IEnumerable<Effect> Active => Effects.Select(e => e.Value).Where(e => e.Active);
        public IEnumerable<Effect> ActiveGroup(string group) => Active.Where(e => string.Equals(e.Group, group));

        public static void Add()
        {
            if (Instance != null) { return; }

            Instance = new CrowdControlHelper(Celeste.Instance);
            Celeste.Instance.Components.Add(Instance);
        }

        ~CrowdControlHelper() => Dispose(false);

        protected override void Dispose(bool disposing)
        {
            try { Log.OnMessage -= OnLogMessage; }
            catch { /**/ }
            base.Dispose(disposing);
        }

        public CrowdControlHelper(Game game) : base(game)
        {
            Log.OnMessage += OnLogMessage;
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
            }

            _client = new SimpleTCPClient();
            _client.RequestReceived += RequestReceived;
        }

        private void OnLogMessage(string s)
        {
            _gui_messages.Enqueue(new GUIMessage { message = s, elapsed = TimeSpan.Zero });
        }

        public static void Remove()
        {
            if (Instance == null) { return; }

            Instance._client.Dispose();

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

        private static readonly TimeSpan MAX_GUI_MESSAGE_TIME = TimeSpan.FromSeconds(2);
        public override void Update(GameTime gameTime)
        {
            _last_time = gameTime;
            base.Update(gameTime);

            // Note: This runs earlier than the game finishes loading!
            if (!(Engine.Scene is GameLoader)) { GameReady = true; }
            if (!GameReady) { return; }

            Player = Engine.Scene?.Tracker?.GetEntity<Player>();

            foreach (Effect action in Active)
            {
                try
                {
                    switch (action.Type)
                    {
                        case Effect.EffectType.Timed:
                            if (action.Elapsed <= action.Duration) { action.Update(gameTime); }
                            else { action.TryStop(); }
                            break;
                        case Effect.EffectType.BidWar:
                            action.Update(gameTime);
                            break;
                        default:
                            action.Update(gameTime);
                            action.TryStop();
                            break;
                    }
                }
                catch (Exception e) { Log.Error(e); }
            }

            while (_gui_messages.Count > MAX_GUI_MESSAGES) { _gui_messages.TryDequeue(out _); }
            foreach (var gm in _gui_messages) { gm.elapsed += gameTime.ElapsedGameTime; }
            while (_gui_messages.TryPeek(out var g) && (g.elapsed > MAX_GUI_MESSAGE_TIME)) { _gui_messages.TryDequeue(out _); }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Note: This runs earlier than the game finishes loading!
            if (!GameReady) { return; }

            Monocle.Draw.SpriteBatch.Begin();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Crowd Control - Connected: {_client.Connected}");
            foreach (var msg in _gui_messages.Take(MAX_GUI_MESSAGES)) { sb.AppendLine(msg.message); }

            ActiveFont.DrawOutline(
                sb.ToString(),
                Vector2.Zero, // Position in "GUI coordinates" (1920 x 1080)
                Vector2.Zero, // "Center point" inside the text (0f - 1f for both x, y)
                Vector2.One * 0.5f, // Scale (x, y)
                Color.White, // Text color
                1f, // Outline width
                Color.Black // Outline color
            );

            foreach (Effect action in Active)
            {
                try { action.Draw(gameTime); }
                catch (Exception e) { Log.Error(e); }
            }
            Monocle.Draw.SpriteBatch.End();
        }

        private void RequestReceived(SimpleTCPClient.Request request)
        {
            //Log.Message($"Got an effect request [{request.id}:{request.code}].");
            if (!Effects.TryGetValue(request.code, out Effect effect))
            {
                //Log.Message($"Effect {request.code} not found.");
                //could not find the effect
                Respond(request, SimpleTCPClient.EffectResult.Unavailable).Forget();
                return;
            }

            int len = effect.ParameterTypes.Length;
            if ((request.parameters?.Length ?? 0) < len)
            {
                Respond(request, SimpleTCPClient.EffectResult.Failure).Forget();
                return;
            }
            object[] p = new object[len];
            for (int i = 0; i < len; i++)
            {
                p[i] = Convert.ChangeType(request.parameters[i], effect.ParameterTypes[i]);
            }


            if (effect.Type == Effect.EffectType.BidWar)
            {
                foreach (Effect e in ActiveGroup(effect.Group)) { e.TryStop(); }
            }
            if (!effect.TryStart(p))
            {
                //Log.Message($"Effect {request.code} could not start.");
                //could not start the effect
                Respond(request, SimpleTCPClient.EffectResult.Retry).Forget();
                return;
            }

            Log.Message($"Effect {request.code} started.");
            Respond(request, SimpleTCPClient.EffectResult.Success).Forget();
        }

        private async Task<bool> Respond(SimpleTCPClient.Request request, SimpleTCPClient.EffectResult result, string message = "")
        {
            try
            {
                return await _client.Respond(new SimpleTCPClient.Response
                {
                    id = request.id,
                    status = result,
                    message = message
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CrowdControl;
using CrowdControl.Client.Binary;
using Microsoft.Xna.Framework;
using Monocle;
using Effect = Celeste.Mod.CrowdControl.Actions.Effect;
using Log = CrowdControl.Log;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlHelper : DrawableGameComponent
    {
        public static CrowdControlHelper Instance;

        private readonly BinaryClient _client;
        private readonly Scheduler _scheduler;

        private readonly ConcurrentQueue<GUIMessage> _gui_messages = new ConcurrentQueue<GUIMessage>();
        private const int MAX_GUI_MESSAGES = 5;

        private static readonly string INITIAL_CONNECT_WARNING = $"This plugin requires the Crowd Control client software.{Environment.NewLine}Please see https://crowdcontrol.live/ for more information.";

        private bool _connected_once = false;

        private class GUIMessage
        {
            public string message;
            public TimeSpan elapsed;
        }

        public bool Connected => _client.ConnectionState == ConnectionStateValue.LoggedIn;

        public bool GameReady = false;

        private GameTime _last_time = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

        public Player Player;

        public readonly Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
        public IEnumerable<Effect> Active => Effects.Select(e => e.Value).Where(e => e.Active);
        public IEnumerable<Effect> ActiveGroup(string group) => Active.Where(e => string.Equals(e.Group, group));

        private Action<string> _token_response;

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
            try { _scheduler.Dispose(); }
            catch { /**/ }
            base.Dispose(disposing);
        }

        public CrowdControlHelper(Game game) : base(game)
        {
#if DEBUG
            Log.OnMessage += OnLogMessage;
#endif
            // Update before and draw after the game.
            UpdateOrder = -10000;
            DrawOrder = 10000;

            // Find all actions and collect them.
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (!typeof(Effect).IsAssignableFrom(type) || type.IsAbstract) { continue; }

                Effect action = (Effect)type.GetConstructor(Everest._EmptyTypeArray).Invoke(Everest._EmptyObjectArray);
                //action.Load();
                Effects.Add(action.Code, action);
            }

            string serviceKey = CrowdControlModule.Settings.ServiceKey;
            if (string.IsNullOrWhiteSpace(serviceKey)) { _client = new BinaryClient(); }
            else { _client = new BinaryClient(serviceKey.ToBytes()); }

            _client.TemporaryTokenPrompt += rf =>
            {
                Log.Debug("Got a temporary token request.");
                _token_response = rf;
            };

            _client.ConnectionStateChanged += s =>
            {
                if (s == ConnectionStateValue.LoggedIn)
                {
                    _connected_once = true;
                    CrowdControlModule.Settings.ServiceKey = _client.LoginToken.ToHexString();
                }
            };

            _scheduler = new Scheduler(_client, Effects.Values);

            _client.Connect(5, "staging-gamesocket.crowdcontrol.live", 27442, true).Forget();
        }

        public void OnUserKeyInput(string key)
        {
            //if (key.Length != 6) { return; }
            Log.Debug("User entered a temporary token: " + key);
            _token_response?.Invoke(key);
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
                            if (action.Elapsed <= action.Duration)
                            {
                                action.Update(gameTime);
                                if ((Engine.Scene is Level level) && level.InCutscene) { action.Elapsed -= gameTime.ElapsedGameTime; }
                            }
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
            if (_client.ConnectionState != ConnectionStateValue.LoggedIn) { sb.AppendLine("Crowd Control - Not Connected"); }
            if (!_connected_once) { sb.AppendLine(INITIAL_CONNECT_WARNING); }
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
    }
}

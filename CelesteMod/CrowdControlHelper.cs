using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlHelper : DrawableGameComponent
    {
        public static CrowdControlHelper Instance;

        private readonly SimpleTCPClient _client;

        private string _gui_message = "";

        public bool GameReady = false;

        public Player Player;

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

            _client = new SimpleTCPClient();
            _client.RequestReceived += (_, e) => RequestReceived(e);
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

        private void RequestReceived(SimpleTCPClient.Request request)
        {
            Log.Message($"Got an effect request [{request.id}:{request.code}].");
            if (!Effects.TryGetValue(request.code, out Effect effect))
            {
                Log.Message($"Effect {request.code} not found.");
                //could not find the effect
                Respond(request, SimpleTCPClient.EffectResult.Unavailable).Forget();
                return;
            }

            if (!effect.TryStart())
            {
                Log.Message($"Effect {request.code} could not start.");
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

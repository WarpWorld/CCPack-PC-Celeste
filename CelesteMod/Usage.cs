using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CrowdControl
{
    public class Example : IDisposable
    {
        private readonly SimpleTCPClient _client = new SimpleTCPClient();

        public Example() => _client.RequestReceived += RequestReceived;

        ~Example() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) { _client.Dispose(); }
        }

        public readonly ConcurrentDictionary<string, Func<SimpleTCPClient.Request, bool>> Effects = new ConcurrentDictionary<string, Func<SimpleTCPClient.Request, bool>>();

        private void RequestReceived(SimpleTCPClient.Request request)
        {
            Log.Message($"Got an effect request [{request.id}:{request.code}].");
            if (!Effects.TryGetValue(request.code, out var effect))
            {
                Log.Message($"Effect {request.code} not found.");
                //could not find the effect
                Respond(request, SimpleTCPClient.EffectResult.Unavailable).Forget();
                return;
            }

            if (!effect.Invoke(request))
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

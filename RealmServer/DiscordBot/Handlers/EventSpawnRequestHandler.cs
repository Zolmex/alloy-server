#region

using Common.API.Requests;
using Newtonsoft.Json;

#endregion

namespace DiscordBot.Handlers
{
    [Request("eventspawn")]
    public class EventSpawnRequestHandler : IRequestHandler
    {
        public bool Handle(string requestJson)
        {
            var eventSpawn = JsonConvert.DeserializeObject<EventSpawnRequest>(requestJson);
            DiscordBotOutput.SendMessage($"{eventSpawn.EventName} has spawned!");
            return true;
        }
    }
}
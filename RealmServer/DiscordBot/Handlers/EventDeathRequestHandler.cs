#region

using Common.API.Requests;
using Newtonsoft.Json;

#endregion

namespace DiscordBot.Handlers
{
    [Request("eventdeath")]
    public class EventDeathRequestHandler : IRequestHandler
    {
        public bool Handle(string requestJson)
        {
            var eventDeath = JsonConvert.DeserializeObject<EventDeathRequest>(requestJson);
            DiscordBotOutput.SendMessage($"{eventDeath.EventName} has died!");
            return true;
        }
    }
}
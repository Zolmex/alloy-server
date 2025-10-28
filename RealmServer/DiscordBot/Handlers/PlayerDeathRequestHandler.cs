#region

using Common.API.Requests;
using Newtonsoft.Json;

#endregion

namespace DiscordBot.Handlers
{
    [Request("playerdeath")]
    public class PlayerDeathRequestHandler : IRequestHandler
    {
        public bool Handle(string requestJson)
        {
            var playerDeath = JsonConvert.DeserializeObject<PlayerDeathRequest>(requestJson);
            DiscordBotOutput.SendMessage($"{playerDeath.PlayerName} was killed by {playerDeath.Killer}!");
            return true;
        }
    }
}
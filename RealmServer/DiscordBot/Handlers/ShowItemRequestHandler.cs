#region

using Common.API.Requests;
using Newtonsoft.Json;

#endregion

namespace DiscordBot.Handlers
{
    [Request("droploot")]
    public class ShowItemRequestHandler : IRequestHandler
    {
        public bool Handle(string requestJson)
        {
            var showItem = JsonConvert.DeserializeObject<DropLootRequest>(requestJson);
            DiscordBotOutput.ShowItem(showItem.ItemName);
            return true;
        }
    }
}
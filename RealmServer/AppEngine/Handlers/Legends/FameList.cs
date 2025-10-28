#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Legends
{
    public class FameList : RequestHandler
    {
        public override string Path => "/fame/list";

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            var timespan = query["timespan"];

            var legendsList = await DbClient.GetLegends(timespan);
            return legendsList?.ToString() ?? WriteError("Invalid legends timespan");
        }
    }
}
#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Legends;

public class FameList : RequestHandler
{
    public override string Path => "/fame/list";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var timespan = query["timespan"];

        // var legendsList = await DbClient.GetLegends(timespan); // TODO: fix
        return /*legendsList?.ToString() ?? */WriteError("Invalid legends timespan");
    }
}
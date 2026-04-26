#region

using System.Collections.Specialized;
using System.Threading.Tasks;
using Common.Database;

#endregion

namespace WebServer.Handlers.Char;

public class Fame : RequestHandler {
    public override string Path => "/char/fame";

    public override async Task<string> Handle(string ip, NameValueCollection query) {
        var accId = int.Parse(query["accountId"]);
        var charId = int.Parse(query["charId"]);

        var fameInfo = await DbClient.GetDeathInfoAsync(accId, charId); // TODO: fix
        return WriteError("No death info available");

        // return fameInfo.ToXml(await DbClient.GetCharacter(accId, charId)).ToString();
    }
}
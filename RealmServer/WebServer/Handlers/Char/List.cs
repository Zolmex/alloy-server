#region

using Common.Database;
using Common.Utilities;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Char;

public class ListMembers : RequestHandler
{
    public override string Path => "/char/list";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClient.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Account ?? Common.Database.Models.Account.Guest;
        return acc.ToCharListXml().ToString();
    }
}
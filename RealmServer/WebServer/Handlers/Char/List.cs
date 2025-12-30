#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Char;

public class ListMembers : RequestHandler
{
    public override string Path => "/char/list";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClientOld.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Item1 ?? DbClientOld.GetGuestAccount();
        return acc.ToCharListXml().ToString();
    }
}
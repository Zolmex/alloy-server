#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Guild;

public class ListMembers : RequestHandler
{
    public override string Path => "/guild/listMembers";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClientOld.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Item1;
        if (acc == null)
            return WriteError("Invalid account credentials.");

        var guild = await DbClientOld.GetGuild(acc.GuildId);
        if (guild == null)
            return WriteError("Invalid guild id.");

        return guild.ToXML().ToString();
    }
}
#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Guild;

public class GetBoard : RequestHandler
{
    public override string Path => "/guild/getBoard";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClient.VerifyAccountAsync(query["username"], query["password"]);

        var acc = verify.Account;
        if (acc == null)
            return WriteError("Invalid account credentials.");

        var guild = await DbClient.GetGuildAsync(acc.GuildMember?.GuildId ?? 0);
        if (guild == null)
            return WriteError("Invalid guild id.");

        return guild.GuildBoard ?? "";
    }
}
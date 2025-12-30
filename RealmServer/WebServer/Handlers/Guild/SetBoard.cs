#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Guild;

public class SetBoard : RequestHandler
{
    public override string Path => "/guild/setBoard";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var board = query["board"];
        if (string.IsNullOrWhiteSpace(board))
            return WriteError("Invalid board text.");

        var verify = await DbClientOld.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Item1;
        var status = verify.Item2;
        if (acc == null)
            return WriteError("Invalid account credentials.");

        var guild = await DbClientOld.GetGuild(acc.GuildId);
        if (guild == null)
            return WriteError("Invalid guild id.");

        guild.Board = board;
        DbClientOld.Save(guild);

        return guild.Board;
    }
}
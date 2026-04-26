#region

using System.Collections.Specialized;
using System.Threading.Tasks;
using Common.Database;

#endregion

namespace WebServer.Handlers.Guild;

public class SetBoard : RequestHandler {
    public override string Path => "/guild/setBoard";

    public override async Task<string> Handle(string ip, NameValueCollection query) {
        var board = query["board"];
        if (string.IsNullOrWhiteSpace(board))
            return WriteError("Invalid board text.");

        var verify = await DbClient.VerifyAccountAsync(query["username"], query["password"]);

        var acc = verify.Account;
        var status = verify.Status;
        if (acc == null)
            return WriteError("Invalid account credentials.");

        var guild = await DbClient.GetGuildAsync(acc.GuildMember?.GuildId ?? 0);
        if (guild == null)
            return WriteError("Invalid guild id.");

        guild.GuildBoard = board;
        await DbClient.FlushAsync(guild, g => g.GuildBoard);

        return guild.GuildBoard;
    }
}
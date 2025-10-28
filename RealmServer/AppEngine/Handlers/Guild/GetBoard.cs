#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Guild
{
    public class GetBoard : RequestHandler
    {
        public override string Path => "/guild/getBoard";

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            var verify = await DbClient.VerifyAccount(query["username"], query["password"]);

            var acc = verify.Item1;
            if (acc == null)
                return WriteError("Invalid account credentials.");

            var guild = await DbClient.GetGuild(acc.GuildId);
            if (guild == null)
                return WriteError("Invalid guild id.");

            return guild.Board ?? "";
        }
    }
}
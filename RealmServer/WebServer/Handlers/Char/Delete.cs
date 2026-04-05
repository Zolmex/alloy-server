#region

using Common.Database;
using Common.Utilities;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Char;

public class Delete : RequestHandler
{
    public override string Path => "/char/delete";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClient.VerifyAccountAsync(query["username"], query["password"]);

        var acc = verify.Account;
        var status = verify.Status;
        if (acc == null)
            return status.GetDescription();

        if (!int.TryParse(query["charId"], out var charId))
            return WriteError("A character Id is required to delete the character");

        if (await DbClient.DeleteCharacterAsync(acc.Id, charId))
            return WriteSuccess();

        return WriteError("Internal server error.");
    }
}
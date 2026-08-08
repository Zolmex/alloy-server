#region

using System.Collections.Specialized;
using System.Threading.Tasks;
using Common;
using Common.Database;
using Common.Utilities;

#endregion

namespace WebServer.Handlers.Account;

public class Register : RequestHandler {
    public override string Path => "/account/register";

    public override async Task<string> Handle(string ip, NameValueCollection query) {
        var result = await DbClient.RegisterAsync(query["newUsername"], query["newPassword"], ip);
        if (result != RegisterStatus.Success)
            return WriteError(result.GetDescription());
        return WriteSuccess();
    }
}
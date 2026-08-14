#region

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Common.Database;
using Common.Utilities;

#endregion

namespace AccountServer.Handlers.Account;

public class Verify : RequestHandler {
    public override string Path => "/account/verify";

    public override async Task<string> Handle(string ip, NameValueCollection query) {
        var verify = DbClient.VerifyAccount(query["username"], query["password"], Guid.Empty);

        var acc = verify.Acc;
        var status = verify.Status;
        if (acc == null)
            return status.GetDescription();

        return acc.ToXml().ToString();
    }
}
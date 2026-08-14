#region

using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Common.Database;
using Common.Utilities;

#endregion

namespace AccountServer.Handlers.Char;

public class ListMembers : RequestHandler {
    public override string Path => "/char/list";

    public override async Task<string> Handle(string ip, NameValueCollection query) {
        var verify = DbClient.VerifyAccount(query["username"], query["password"], Guid.Empty);

        var acc = verify.Acc ?? Common.Database.Models.Account.Guest;
        return acc.ToCharListXml().ToString();
    }
}
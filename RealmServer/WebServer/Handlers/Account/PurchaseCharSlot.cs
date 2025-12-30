#region

using Common.Database;
using Common.Utilities;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace WebServer.Handlers.Account;

public class PurchaseCharSlot : RequestHandler
{
    public override string Path => "/account/purchaseCharSlot";

    public override async Task<string> Handle(string ip, NameValueCollection query)
    {
        var verify = await DbClientOld.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Item1;
        var status = verify.Item2;
        if (acc == null)
            return status.GetDescription();

        DbClientOld.BuyCharSlot(acc);
        return WriteSuccess();
    }
}
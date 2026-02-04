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
        var verify = await DbClient.VerifyAccount(query["username"], query["password"]);

        var acc = verify.Account;
        var status = verify.Status;
        if (acc == null)
            return status.GetDescription();

        await DbClient.BuyCharSlot(acc);
        return WriteSuccess();
    }
}
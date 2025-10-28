#region

using Common.Database;
using Common.Utilities;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Char
{
    public class Delete : RequestHandler
    {
        public override string Path => "/char/delete";

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            var verify = await DbClient.VerifyAccount(query["username"], query["password"]);

            var acc = verify.Item1;
            var status = verify.Item2;
            if (acc == null)
                return status.GetDescription();

            if (!int.TryParse(query["charId"], out var charId))
                return WriteError("A character Id is required to delete the character");

            var chr = await DbClient.GetChar(acc.AccountId, charId);
            if (chr == null)
                return WriteError("Invalid character Id.");

            if (await DbClient.DeleteChar(acc, chr))
                return WriteSuccess();

            return WriteError("Internal server error.");
        }
    }
}
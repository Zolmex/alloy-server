#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Char
{
    public class ListMembers : RequestHandler
    {
        public override string Path => "/char/list";

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            var verify = await DbClient.VerifyAccount(query["username"], query["password"]);

            var acc = verify.Item1 ?? DbClient.GetGuestAccount();
            return acc.ToCharListXml().ToString();
        }
    }
}
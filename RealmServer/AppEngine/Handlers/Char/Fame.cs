#region

using Common.Database;
using System.Collections.Specialized;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Char
{
    public class Fame : RequestHandler
    {
        public override string Path => "/char/fame";

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            var accId = int.Parse(query["accountId"]);
            var charId = int.Parse(query["charId"]);

            var fameInfo = await DbClient.GetDeathInfo(accId, charId);
            if (fameInfo == null)
                return WriteError("No death info available");

            return fameInfo.ToXml(await DbClient.GetChar(accId, charId)).ToString();
        }
    }
}
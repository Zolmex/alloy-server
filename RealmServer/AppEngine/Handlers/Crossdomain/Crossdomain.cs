#region

using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace AppEngine.Handlers.Crossdomain
{
    public class Crossdomain : RequestHandler
    {
        private const string CROSSDOMAIN_PATH = "Handlers/Crossdomain/crossdomain.xml";

        public override string Path => "/crossdomain.xml";
        private string _file;

        public override async Task<string> Handle(string ip, NameValueCollection query)
        {
            if (_file == null)
                _file = File.ReadAllText(CROSSDOMAIN_PATH);

            return _file;
        }
    }
}
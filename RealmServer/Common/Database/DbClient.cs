using Common.Network;
using Common.Resources.Config;
using Common.Utilities;
using System.Threading.Tasks;

namespace Common.Database;

public static class DbClient
{
    private static readonly Logger _log = new(typeof(DbClient));
    private static readonly AppConnection _con = new();
    
    public static async Task Connect(DatabaseConfig config)
    {
        _con.SetupNew();
        await _con.Connect(config.Host, config.Port);
        
        _log.Info("Connected to DbServer successfully.");
    }
}
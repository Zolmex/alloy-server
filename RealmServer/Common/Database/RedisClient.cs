#region

using Common.Resources.Config;
using Common.Utilities;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace Common.Database
{
    public static class RedisClient
    {
        private static ILogger _log = new Logger(typeof(RedisClient));

        public static IDatabase Db { get; private set; }
        public static ISubscriber Sub { get; private set; }
        public static IServer Server { get; private set; }

        private static ConnectionMultiplexer _conn;


        public static void Connect(DatabaseConfig config)
        {
            if (_conn is not null && _conn.IsConnected)
            {
                _log.Warn("Already connected to Redis Server.");
                return;
            }

            var conString = config.Host + ":" + config.Port + ",syncTimeout=30000"; // Build the configuration string to connect to a specific Redis server
            if (!string.IsNullOrWhiteSpace(config.Password))
                conString += ",password=" + config.Password;

            _log.Info($"Connecting to Redis @ {config.Host}:{config.Port}");

            _conn = ConnectionMultiplexer.Connect(conString); // Connect to the Redis server and set up static fields.

            Db = _conn.GetDatabase(config.DbIndex);
            Sub = _conn.GetSubscriber();

            Server = _conn.GetServer(_conn.GetEndPoints().First());

            _log.Info($"Connected database to index {config.DbIndex}");
        }

        public static async Task ConnectAsync(DatabaseConfig config)
        {
            if (_conn is not null && _conn.IsConnected)
            {
                _log.Warn("Already connected to Redis Server.");
                return;
            }

            var conString = config.Host + ":" + config.Port + ",syncTimeout=30000,allowAdmin=true"; // Build the configuration string to connect to a specific Redis server
            if (!string.IsNullOrWhiteSpace(config.Password))
                conString += ",password=" + config.Password;

            _log.Info($"Connecting to Redis @ {config.Host}:{config.Port}");

            _conn = await ConnectionMultiplexer.ConnectAsync(conString);

            Db = _conn.GetDatabase(config.DbIndex);
            Sub = _conn.GetSubscriber();

            _log.Info($"Connected database to index {config.DbIndex}");
        }

        public static void SetLogger(ILogger logger)
        {
            _log = logger;
        }
    }
}
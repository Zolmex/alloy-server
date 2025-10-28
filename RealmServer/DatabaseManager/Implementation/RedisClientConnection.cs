#region

using Common.Database;
using Common.Resources.Config;
using Common.Utilities;
using DatabaseManager.Interface;
using StackExchange.Redis;

#endregion

namespace DatabaseManager.Implementation
{
    public class RedisClientConnection : IRedisConnection
    {
        private readonly ILogger _logger = new Logger(typeof(RedisClientConnection));

        private IDatabase? _db;
        private IConnectionMultiplexer? _multiplexer;
        private IServer? _server;

        public bool IsConnected => _multiplexer is not null && _db is not null && _multiplexer.IsConnected;

        public RedisClientConnection()
        {
            RedisClient.SetLogger(_logger);
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            await RedisClient.ConnectAsync(DatabaseConfig.Config);

            _db = RedisClient.Db;
            _multiplexer = _db.Multiplexer;
            _server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
        }

        public void Disconnect()
        {
            _server?.Shutdown();
        }

        public async Task SaveAsync()
        {
            if (_server is null)
            {
                _logger.Error("Could not save database: Database not connected.");
                return;
            }

            await _server.SaveAsync(SaveType.BackgroundSave);
        }

        private Dictionary<string, byte[]> _keysCache = new();

        public async Task WipeAsync()
        {
            if (_server is null || _db is null)
            {
                _logger.Error("Could not wipe database: Database not connected.");
                return;
            }

            // Get all keys in set
            var keepOnWipeKeys = await _db.SetMembersAsync(DbKeys.KeepOnWipe);

            _keysCache = new Dictionary<string, byte[]>();
            foreach (var key in keepOnWipeKeys)
            {
                _keysCache.Add(key, await _db.KeyDumpAsync((string)key));
            }

            await _server.FlushDatabaseAsync(RedisClient.Db.Database);

            foreach (var (key, value) in _keysCache)
            {
                await _db.KeyRestoreAsync(key, value);
            }
        }

        public async Task<DateTime> GetLastSaveAsync()
        {
            if (_server is null)
            {
                _logger.Error("Could not get last save time: Database not connected.");
                return DateTime.MinValue;
            }

            return await _server.LastSaveAsync();
        }

        public async Task<RedisResult> ExecuteCommandAsync(string command)
        {
            if (!IsConnected)
            {
                _logger.Error("Could not execute command async: Database not connected.");
                return RedisResult.Create(RedisValue.Null);
            }

            return await _db.ExecuteAsync(command);
        }

        public async Task<string> GetRdbFilePathAsync()
        {
            if (_server is null)
            {
                _logger.Error("Could not get last save time: Database not connected.");
                return string.Empty;
            }

            var info = await _server.InfoAsync("server");
            return info.First().FirstOrDefault(x => x.Key == "executable").Value;
        }
    }
}
#region

using StackExchange.Redis;

#endregion

namespace DatabaseManager.Interface
{
    public interface IRedisConnection
    {
        bool IsConnected { get; }

        Task ConnectAsync();
        void Disconnect();
        Task SaveAsync();
        Task WipeAsync();
        Task<DateTime> GetLastSaveAsync();
        Task<RedisResult> ExecuteCommandAsync(string command);
        Task<string> GetRdbFilePathAsync();
    }
}
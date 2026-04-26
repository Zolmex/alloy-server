using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;

namespace Common.Database;

public static class DbClient {
    private static readonly Logger _log = new(typeof(DbClient));
    private static readonly AppConnection _con = new();

    public static async Task ConnectAsync(DatabaseConfig config) {
        _con.SetupNew();
        await _con.Connect(config.Host, config.Port);

        _log.Info("Connected to DbServer successfully.");
    }

    public static bool IsValidUsername(string name) {
        return !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);
    }

    public static bool IsValidPassword(string password) {
        return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
    }

    public static async Task<RegisterStatus> RegisterAsync(string username, string password, string ip) {
        if (!IsValidUsername(username))
            return RegisterStatus.InvalidName;
        if (!IsValidPassword(password))
            return RegisterStatus.InvalidPassword;

        var ack = await _con.SendAndReceiveAsync<RegisterAck>(
            new RegisterMessage { Username = username, IPAddress = ip, Password = password });
        return ack.Status;
    }

    public static async Task<VerifyAck> VerifyAccountAsync(string username, string password) {
        var ack = await _con.SendAndReceiveAsync<VerifyAck>(
            new VerifyMessage { Username = username, Password = password });
        return ack;
    }

    public static async Task BuyCharSlotAsync(Account acc) {
        var cost = NewAccountsConfig.Config.CharSlotCost;
        Logger.Debug($"{acc.AccStats?.CurrentFame ?? 420}:{cost}");
        if (acc.AccStats?.CurrentFame < cost)
            return;

        acc.AccStats!.CurrentFame -= (uint)cost;
        acc.MaxChars++;

        await FlushAsync(acc.AccStats, stats => stats.CurrentFame);
        await FlushAsync(acc, a => a.MaxChars);
    }

    public static async Task<GetCharacterAck> GetCharacterAsync(int accId, int accCharId) {
        var ack = await _con.SendAndReceiveAsync<GetCharacterAck>(
            new GetCharacterMessage { AccountId = accId, CharacterId = accCharId });
        return ack;
    }

    public static async IAsyncEnumerable<IAppMessageAck> FlushAsync(params DbModel[] models) {
        foreach (var model in models)
            yield return await model.FlushAll(_con);
    }

    public static async Task<FlushAck> FlushAsync<T, TValue>(T model, params Expression<Func<T, TValue>>[] expressions)
        where T : DbModel {
        return await model.Flush(_con, expressions);
    }

    public static async Task<CreateCharacterAck> CreateCharacterAsync(Account acc, ushort objectType, ushort skinType) {
        var ack = await _con.SendAndReceiveAsync<CreateCharacterAck>(
            new CreateCharacterMessage { AccountId = acc.Id, ClassType = objectType, SkinType = skinType });
        return ack;
    }

    public static async Task<bool> DeleteCharacterAsync(int accId, int accCharId) {
        var ack = await _con.SendAndReceiveAsync<DeleteCharacterAck>(
            new DeleteCharacterMessage { AccountId = accId, CharacterId = accCharId });
        return ack.Success;
    }

    public static async Task<Account> GetAccountByNameAsync(string name) {
        var ack = await _con.SendAndReceiveAsync<GetAccountAck>(
            new GetAccountByNameMessage { Name = name });
        return ack.Account;
    }

    public static async Task<Account> GetAccountAsync(int accId) {
        var ack = await _con.SendAndReceiveAsync<GetAccountAck>(
            new GetAccountByNameMessage { AccountId = accId });
        return ack.Account;
    }

    public static async Task<BanAccountAck> BanAccountAsync(string name, string reason, DateTime expiresAt,
        int moderatorId) {
        var ack = await _con.SendAndReceiveAsync<BanAccountAck>(
            new BanAccountMessage {
                Name = name,
                Reason = reason,
                ExpiresAt = expiresAt,
                ModeratorId = moderatorId
            });
        return ack;
    }

    public static async Task<UnbanAccountAck> UnbanAccountAsync(string name) {
        var ack = await _con.SendAndReceiveAsync<UnbanAccountAck>(
            new UnbanAccountMessage { Name = name });
        return ack;
    }

    public static async Task<(bool, string)>
        MuteAccountAsync(string name) // TODO: proper muted model with "reason" and "muted by" fields
    {
        var acc = await GetAccountByNameAsync(name);
        if (acc == null)
            return (false, $"Account {name} not found.");

        if (acc.IsMuted) // If this is true, we need to check if the mute has expired
            if (acc.AccountMutes.Any(b => b.ExpiresAt == null) || // Means the account is permanently muted
                acc.AccountMutes.Any(b => b.ExpiresAt > DateTime.UtcNow)) // Means the mute hasn't been lifted yet
                return (false, $"Account {name} is already muted.");

        // Account's mute was lifted, continue...
        acc.IsMuted = true;

        await acc.Flush<Account, bool>(_con, a => a.IsMuted);
        return (true, "");
    }

    public static async Task<(bool, string)> UnmuteAccountAsync(string name) {
        var acc = await GetAccountByNameAsync(name);
        if (acc == null)
            return (false, $"Account {name} not found.");

        if (!acc.IsMuted)
            return (false, $"Account {name} is not muted.");

        acc.IsMuted = false;

        await acc.Flush<Account, bool>(_con, a => a.IsMuted);
        return (true, "");
    }

    public static async Task<CharacterDeath> GetDeathInfoAsync(int accId, int charId) {
        return null;
    }

    public static async Task<Guild> GetGuildAsync(int guildId) {
        return null;
    }
}
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;
using System;
using System.Linq;
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

    public static bool IsValidUsername(string name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length > 0 && name.Length < 11 && name.All(char.IsLetter);
    }

    public static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length > 8;
    }

    public static async Task<RegisterStatus> Register(string username, string password, string ip)
    {
        if (!IsValidUsername(username))
            return RegisterStatus.InvalidName;
        if (!IsValidPassword(password))
            return RegisterStatus.InvalidPassword;

        var ack = (RegisterAck)await _con.SendAndReceiveAsync(
            new RegisterMessage
            {
                Username = username,
                IPAddress = ip,
                Password = password
            });
        return ack.Status;
    }

    public static async Task<VerifyAck> VerifyAccount(string username, string password)
    {
        var ack = (VerifyAck)await _con.SendAndReceiveAsync(
            new VerifyMessage
            {
                Username = username,
                Password = password
            });
        return ack;
    }
    
    public static async Task BuyCharSlot(Account acc)
    {
        var cost = NewAccountsConfig.Config.CharSlotCost;
        if (acc.AccStats?.CurrentFame < cost)
            return;

        acc.AccStats!.CurrentFame = (uint)Math.Max(0, acc.AccStats.CurrentFame.GetValueOrDefault() - cost);
        acc.MaxChars++;
        
        await acc.AccStats.Flush<AccountStat>(_con, stats => stats.CurrentFame);
        await acc.Flush<Account>(_con, a => a.MaxChars);
    }
}
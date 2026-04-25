using Common;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using DbServer.Database;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Messaging;

public class BanAccountHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.BanAccount;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (BanAccountMessage)msg;
        var response = new BanAccountAck(pkt.Sequence);
        Logger.Debug($"BanAccount: {pkt.Name}");

        var success = true;
        var error = "";

        var acc = await DbCache.Accounts.FirstOrDefaultAsync(a => a.Name == pkt.Name);
        if (acc == null)
        {
            success = false;
            error = $"Account {pkt.Name} not found";
        }
        else if (acc.IsBanned) // If this is true, we need to check if the ban has expired
        {
            if (acc.AccountBans.Any(b => b.ExpiresAt == null) || // Means the account is permanently banned
                acc.AccountBans.Any(b => b.ExpiresAt > DateTime.UtcNow)) // Means the ban hasn't been lifted yet
            {
                success = false;
                error = $"Account {pkt.Name} is already banned.";
            }

            // Account's ban was lifted, continue...
        }

        response.Success = success;
        response.Error = error;

        if (!success)
        {
            con.Send(response);
            return;
        }

        var ban = new AccountBan()
        {
            Reason = pkt.Reason,
            ExpiresAt = pkt.ExpiresAt,
            ModeratorId = pkt.ModeratorId,
        };
        acc.IsBanned = true;
        acc.AccountBans.Add(ban);
        
        DbCache.Accounts.Update(acc,
            a => a.IsBanned);
        await DbCache.AccountBans.AddAsync(ban);

        await DbCache.SaveChanges(); // Save changes to database

        con.Send(response);
    }
}
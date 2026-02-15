using Common;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Resources.Xml;
using Common.Utilities;
using DbServer.Database;
using DbServer.Implementation;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Messaging;

public class UnbanAccountHandler : IMessageHandler
{
    private static readonly Logger _log = new(typeof(UnbanAccountHandler));
    
    public AppMessageId MessageId => AppMessageId.UnbanAccount;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (UnbanAccountMessage)msg;
        var response = new UnbanAccountAck(pkt.Sequence);
        Logger.Debug($"UnbanAccount: {pkt.Name}");

        var success = true;
        var error = "";

        var acc = await DbCache.Accounts.FirstOrDefaultAsync(a => a.Name == pkt.Name);
        if (acc == null)
        {
            success = false;
            error = $"Account {pkt.Name} not found";
        }
        else if (!acc.IsBanned)
        {
            success = false;
            error = $"Account {pkt.Name} is not banned.";
        }

        response.Success = success;
        response.Error = error;

        if (!success)
        {
            con.Send(response);
            return;
        }
        
        var perma = acc.AccountBans.FirstOrDefault(b => b.ExpiresAt == null); // There should only be one perma ban at a given time
        var activeBan = perma ?? acc.AccountBans.FirstOrDefault(b => b.ExpiresAt > DateTime.UtcNow);

        if (activeBan == null)
        {
            response.Success = false;
            response.Error = "Internal error: no valid ban entry found.";
            con.Send(response);
            _log.Error($"Unban: No valid ban entry found. ({acc.Name}:{acc.Id})");
            return;
        }

        acc.IsBanned = false;
        activeBan.Enabled = false;
        
        DbCache.Accounts.Update(acc, a => a.IsBanned);
        DbCache.AccountBans.Update(activeBan, b => b.Enabled);
        
        await DbCache.SaveChanges(); // Save changes to database

        con.Send(response);
    }
}
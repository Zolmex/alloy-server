using Common;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;
using DbServer.Database;
using DbServer.Implementation;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Messaging;

public class VerifyHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Verify;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (VerifyMessage)msg;
        var response = new VerifyAck(pkt.Sequence);
        Logger.Debug($"Verify: {pkt.Username}:{pkt.Password}");

        var status = VerifyStatus.Success;

        var login = await DbCache.Logins.FirstOrDefaultAsync(l => l.Name == pkt.Username);
        if (login == null)
        {
            status = VerifyStatus.InvalidCredentials;
            response.Status = status;
            con.Send(response);
            return;
        }

        var hash = (pkt.Password + login.PasswordSalt).ToSHA1();
        if (login.PasswordHash != hash)
        {
            response.Status = VerifyStatus.InvalidCredentials;
            con.Send(response);
            return;
        }

        var acc = await DbCache.Accounts.FirstOrDefaultAsync(acc => acc.LoginId == login.Id);
        if (acc == null)
        {
            response.Status = VerifyStatus.InternalError;
            con.Send(response);
            return;
        }

        Logger.Debug($"accStats: {acc.AccStats?.GetHashCode()}");
        
        response.Status = status;
        response.Account = acc;
        con.Send(response);
    }
}
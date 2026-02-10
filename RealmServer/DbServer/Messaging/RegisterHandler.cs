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

public class RegisterHandler : IMessageHandler
{
    private const int MAX_ACCOUNTS_PER_IP = 3000;

    public AppMessageId MessageId => AppMessageId.Register;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (RegisterMessage)msg;
        Logger.Debug($"Register: {pkt.Username}:{pkt.Password}");

        var status = RegisterStatus.Success;

        var lowerName = pkt.Username.ToLower();
        
        // Check name in use
        if (await DbCache.Logins.AnyAsync(i => i.Name.Equals(lowerName)))
            status = RegisterStatus.NameInUse;

        // Check accounts per ip
        // TODO: using Count() is very expensive, replace with a db field
        else if (await DbCache.Logins.CountAsync(i => i.IPAddress == pkt.IPAddress) >= MAX_ACCOUNTS_PER_IP)
            status = RegisterStatus.MaxAccountsReached;

        if (status == RegisterStatus.Success)
        {
            // Used for password encryption
            var salt = MathUtils.GenerateSalt();

            var acc = new Account()
            {
                Name = pkt.Username,
                NextCharId = 1,
                MaxChars = (short)NewAccountsConfig.Config.MaxChars,
                VaultCount = (short)NewAccountsConfig.Config.VaultCount,
                AccStats = new AccountStat()
                {
                    CurrentCredits = (uint)NewAccountsConfig.Config.Credits,
                    TotalCredits = (uint)NewAccountsConfig.Config.Credits,
                    CurrentFame = (uint)NewAccountsConfig.Config.Fame,
                    TotalFame = (uint)NewAccountsConfig.Config.Fame,
                    ClassStats = NewAccountsConfig.CreateClassStats()
                },
                Login = new Login() { Name = lowerName, IPAddress = pkt.IPAddress, PasswordHash = (pkt.Password + salt).ToSHA1(), PasswordSalt = salt },
            };

            await DbCache.Accounts.AddAsync(acc);
            
            var result = await DbCache.SaveChanges();

            if (result == 0) // No entries were added
                status = RegisterStatus.InternalError;
        }

        con.Send(new RegisterAck(pkt.Sequence) { Status = status });
    }
}
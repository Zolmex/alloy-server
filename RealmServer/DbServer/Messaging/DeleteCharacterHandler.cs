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

public class DeleteCharacterHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.DeleteCharacter;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (DeleteCharacterMessage)msg;
        var response = new DeleteCharacterAck(pkt.Sequence);
        Logger.Debug($"DeleteCharacter: {pkt.AccountId}|{pkt.CharacterId}");

        var success = true;

        var acc = await DbCache.Accounts.GetAsync(Account.BuildKey(pkt.AccountId));
        if (acc == null)
        {
            success = false;
        }
        else
        {
            var chr = acc.Characters.FirstOrDefault(c => c.AccCharId == pkt.CharacterId);
            if (chr == null)
            {
                success = false;
            }
            else
            {
                // Perform a "soft" delete, doesn't actually delete from database, instead we mark it as deleted
                chr.IsDeleted = true;
                acc.Characters.Remove(chr); // We do gotta remove the character from the cached account char list

                DbCache.Characters.Update(chr, c => c.IsDeleted);

                await DbCache.SaveChanges(); // Save changes to database
            }
        }

        response.Success = success;
        con.Send(response);
    }
}
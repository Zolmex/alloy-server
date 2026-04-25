using Common;
using Common.Database.Models;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Resources.Config;
using Common.Utilities;
using DbServer.Database;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbServer.Messaging;

public class GetCharacterHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.GetCharacter;

    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (GetCharacterMessage)msg;
        var response = new GetCharacterAck(pkt.Sequence);
        Logger.Debug($"GetCharacter: {pkt.AccountId}:{pkt.CharacterId}");
        
        var chr = await DbCache.Characters.FirstOrDefaultAsync(c => c.AccId == pkt.AccountId && c.AccCharId == pkt.CharacterId);

        Logger.Debug($"Character null?:{chr == null}");
        response.Character = chr;
        con.Send(response);
    }
}
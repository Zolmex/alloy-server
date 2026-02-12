using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class GetAccountAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.GetAccount;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (GetAccountAck)msg;
        Logger.Debug($"GetAccountByName:{pkt.Account != null}");
    }
}
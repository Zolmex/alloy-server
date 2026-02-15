using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class BanAccountAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.BanAccount;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (BanAccountAck)msg;
        Logger.Debug($"BanAccount:{pkt.Success}|{pkt.Error}");
    }
}
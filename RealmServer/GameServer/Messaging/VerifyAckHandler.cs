using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class VerifyAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Verify;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (VerifyAck)msg;
        Logger.Debug($"Verify:{pkt.Status}");
    }
}
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class FlushAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Flush;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (FlushAck)msg;
        Logger.Debug($"Flush:{pkt.Status}");
    }
}
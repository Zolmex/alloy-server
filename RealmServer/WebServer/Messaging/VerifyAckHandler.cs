using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace WebServer.Messaging;

public class VerifyAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Verify;
    
    public void Handle(IAppMessage msg, AppConnection con)
    {
        var pkt = (VerifyMessageAck)msg;
        Logger.Debug($"Verify:{pkt.Status}");
    }
}
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace WebServer.Messaging;

public class RegisterAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Register;
    
    public async Task Handle(IAppMessage msg, AppConnection con)
    {
        var pkt = (RegisterAck)msg;
        Logger.Debug($"Register:{pkt.Status}");
    }
}
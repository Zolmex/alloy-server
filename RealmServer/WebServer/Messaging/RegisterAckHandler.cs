using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace WebServer.Messaging;

public class RegisterAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Register;
    
    public void Handle(IAppMessage msg, AppConnection con)
    {
        var pkt = (RegisterMessageAck)msg;
        Logger.Debug($"Register:{pkt.Status}");
    }
}
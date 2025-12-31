using Common;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;

namespace DbServer.Messaging;

public class RegisterHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.Register;
    
    public void Handle(IAppMessage msg, AppConnection con)
    {
        var pkt = (RegisterMessage)msg;
        Logger.Debug($"Register: {pkt.Username}:{pkt.Password}");

        var status = RegisterStatus.Success;
        con.Send(new RegisterMessageAck()
        {
            Sequence = pkt.Sequence,
            Status = status
        });
    }
}
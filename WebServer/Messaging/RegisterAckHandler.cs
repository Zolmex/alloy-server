using System.Threading.Tasks;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;

namespace WebServer.Messaging;

public class RegisterAckHandler : IMessageHandler {
    public AppMessageId MessageId => AppMessageId.Register;

    public async Task HandleAsync(IAppMessage msg, AppConnection con) {
        var pkt = (RegisterAck)msg;
        Logger.Debug($"Register:{pkt.Status}");
    }
}
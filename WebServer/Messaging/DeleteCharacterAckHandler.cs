using System.Threading.Tasks;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;

namespace WebServer.Messaging;

public class DeleteCharacterAckHandler : IMessageHandler {
    public AppMessageId MessageId => AppMessageId.DeleteCharacter;

    public async Task HandleAsync(IAppMessage msg, AppConnection con) {
        var pkt = (DeleteCharacterAck)msg;
        Logger.Debug($"DeleteCharacter:{pkt.Success}");
    }
}
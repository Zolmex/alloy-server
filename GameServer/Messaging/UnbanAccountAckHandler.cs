using System.Threading.Tasks;
using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;

namespace GameServerOld.Messaging;

public class UnbanAccountAckHandler : IMessageHandler {
    public AppMessageId MessageId => AppMessageId.UnbanAccount;

    public async Task HandleAsync(IAppMessage msg, AppConnection con) {
        var pkt = (UnbanAccountAck)msg;
        Logger.Debug($"UnbanAccount:{pkt.Success}|{pkt.Error}");
    }
}
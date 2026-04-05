using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class CreateCharacterAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.CreateCharacter;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (CreateCharacterAck)msg;
        Logger.Debug($"CreateCharacter:{pkt.Status}|{pkt.Character == null}");
    }
}
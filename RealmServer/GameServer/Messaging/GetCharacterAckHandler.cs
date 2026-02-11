using Common.Network;
using Common.Network.Messaging;
using Common.Network.Messaging.Impl;
using Common.Utilities;
using System.Threading.Tasks;

namespace GameServer.Messaging;

public class GetCharacterAckHandler : IMessageHandler
{
    public AppMessageId MessageId => AppMessageId.GetCharacter;
    
    public async Task HandleAsync(IAppMessage msg, AppConnection con)
    {
        var pkt = (GetCharacterAck)msg;
        Logger.Debug($"GetCharacter:{pkt.Character == null}");
    }
}
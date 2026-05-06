using Common.Network;
using GameServer.Game.Entities.Extensions;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERTEXT)]
public record PlayerText : IIncomingPacket {
    public string Text;

    public async Task Handle(User user) {
        user.GameInfo.Player.Speak(user.GameInfo.World, Text);
    }

    public void Read(ref SpanReader rdr) {
        Text = rdr.ReadUTF();
    }
}
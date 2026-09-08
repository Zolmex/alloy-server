using Common.Network;
using GameServer.Game.Entities.Old.Extensions;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERTEXT)]
public record PlayerText : IIncomingPacket {
    public string Text;

    public async Task Handle(User user) {
        user.Session.Player.Speak(user.Session.World, Text);
    }

    public void Read(ref SpanReader rdr) {
        Text = rdr.ReadUTF();
    }
}
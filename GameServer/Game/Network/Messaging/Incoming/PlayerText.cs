using Common.Network;
using GameServer.Game.Chat;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERTEXT)]
public record PlayerText : IIncomingPacket {
    public string Text;

    public async Task Handle(User user) {
        ChatManager.Speak(user.Session.Player, user.Session.World, Text);
    }

    public void Read(ref SpanReader rdr) {
        Text = rdr.ReadUTF();
    }
}
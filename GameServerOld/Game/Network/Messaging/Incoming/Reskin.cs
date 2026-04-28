using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.RESKIN)]
public record Reskin : IIncomingPacket {
    public int SkinType;

    public void Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        user.GameInfo.Player.Skin = (ushort)SkinType;
    }

    public void Read(ref SpanReader rdr) {
        SkinType = rdr.ReadInt32();
    }
}
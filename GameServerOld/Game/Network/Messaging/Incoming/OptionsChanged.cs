using Common.Network;

namespace GameServerOld.Game.Network.Messaging.Incoming;

[Packet(PacketId.OPTIONSCHANGED)]
public record OptionsChanged : IIncomingPacket {
    public byte AllyDamage;
    public byte AllyEntities;
    public byte AllyNotifs;
    public byte AllyParticles;
    public byte AllyShots;

    public void Handle(User user) {
        if (user.GameInfo.State != GameState.Playing)
            return;

        user.GameInfo.AllySettings(AllyShots, AllyDamage, AllyNotifs, AllyParticles, AllyEntities);
    }

    public void Read(ref SpanReader rdr) {
        AllyDamage = rdr.ReadByte();
        AllyEntities = rdr.ReadByte();
        AllyNotifs = rdr.ReadByte();
        AllyParticles = rdr.ReadByte();
        AllyShots = rdr.ReadByte();
    }
}
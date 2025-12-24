#region

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.OPTIONSCHANGED)]
public partial record OptionsChanged : IIncomingPacket
{
    public byte AllyDamage;
    public byte AllyEntities;
    public byte AllyNotifs;
    public byte AllyParticles;
    public byte AllyShots;
    public byte DamageCounter;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        user.GameInfo.AllySettings(AllyShots, AllyDamage, AllyNotifs, AllyParticles, AllyEntities);
        user.GameInfo.UiSettings(DamageCounter);
    }
}
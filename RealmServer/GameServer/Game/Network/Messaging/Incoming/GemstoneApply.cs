#region

#endregion

using Common.Network;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.GEMSTONEAPPLY)]
public partial record GemstoneApply : IIncomingPacket
{
    public byte GemSlot;
    public byte InvSlot;
    public byte Slot;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var player = user.GameInfo.Player;
        player.Inventory.ApplyGemstones(Slot, GemSlot, InvSlot);
    }

    public void Read(NetworkReader rdr)
    {
        Slot = rdr.ReadByte();
        GemSlot = rdr.ReadByte();
        InvSlot = rdr.ReadByte();
    }
}
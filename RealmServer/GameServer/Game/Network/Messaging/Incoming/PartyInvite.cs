#region

#endregion

using Common.Network;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PARTYINVITE)]
public partial record PartyInvite : IIncomingPacket
{
    private int _objId;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var acc = user.Account;

        // Shouldn't happen but make sure player is in party before inviting
        if (user.GameInfo.Player.PartyId == -1)
            return;

        var target = user.GameInfo.Player.World.GetPlayerById(_objId);
        target?.InviteToParty(user.GameInfo.Player);
    }

    public void Read(NetworkReader rdr)
    {
        _objId = rdr.ReadInt32();
    }
}
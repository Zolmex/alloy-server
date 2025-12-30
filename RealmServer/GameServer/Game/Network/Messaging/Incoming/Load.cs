#region

using Common.Database;
using Common.Network;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.LOAD)]
public partial record Load : IIncomingPacket
{
    public int CharId;

    public async void Handle(User user)
    {
        if (user.Account.Banned)
        {
            user.SendFailure(Failure.DEFAULT, "Account has been banned.");
            return;
        }

        var chr = user.GameInfo.Char;
        if (user.State != ConnectionState.Reconnecting)
        {
            chr = await DbClientOld.GetChar(user.Account.AccountId, CharId);
            if (chr == null)
            {
                user.SendFailure(Failure.DEFAULT, $"Failed to load character #{CharId}");
                return;
            }
        }

        if (chr == null)
        {
            user.SendFailure(Failure.DEFAULT, "Invalid reconnect state.");
            return;
        }

        if (chr.Dead)
        {
            user.SendFailure(Failure.DEFAULT, "Character is dead.");
            return;
        }

        var world = user.GameInfo.World;
        if (world == null || world.Deleted || !world.Active)
        {
            user.SendFailure(Failure.DEFAULT, "Invalid world.");
            return;
        }

        user.Load(chr, world);
    }

    public void Read(NetworkReader rdr)
    {
        CharId = rdr.ReadInt32();
    }
}
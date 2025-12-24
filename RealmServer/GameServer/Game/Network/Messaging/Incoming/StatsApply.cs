#region

using Common;
using Common.Network;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.STATSAPPLY)]
public partial record StatsApply : IIncomingPacket
{
    public int[] AllocatedPoints;
    public int SpentPoints;

    public void Handle(User user)
    {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        var fail = SpentPoints > player.StatPoints || SpentPoints <= 0 || player.StatPoints - SpentPoints < 0;

        for (var i = 0; i < 4; i++)
            if (AllocatedPoints[i] < 0)
                fail = true;

        if (fail)
            user.SendPacket(new StatsApplyResult(false));
        else
        {
            player.StatPoints -= SpentPoints;

            player.Char.MainStats[StatType.Attack] += AllocatedPoints[0];
            player.Char.MainStats[StatType.Defense] += AllocatedPoints[1];
            player.Char.MainStats[StatType.Dexterity] += AllocatedPoints[2];
            player.Char.MainStats[StatType.Wisdom] += AllocatedPoints[3];
            player.RecalculateStats();
            player.SaveCharacter(true);

            user.SendPacket(new StatsApplyResult(true));
        }
    }

    public void Read(NetworkReader rdr)
    {
        AllocatedPoints = new int[4];
        var spentPoints = 0;
        for (var i = 0; i < 4; i++)
        {
            AllocatedPoints[i] = rdr.ReadInt32();
            spentPoints += AllocatedPoints[i];
        }

        SpentPoints = spentPoints;
    }
}
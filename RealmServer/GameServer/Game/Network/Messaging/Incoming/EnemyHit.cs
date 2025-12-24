#region

using Common;
using Common.Network;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.ENEMYHIT)]
public partial record EnemyHit : IIncomingPacket
{
    public int Elapsed;
    public int ProjectileId;
    public int TargetId;
    public WorldPosData TargetPos;

    public void Handle(User user)
    {
        if (user.GameInfo.State != GameState.Playing)
            return;

        var player = user.GameInfo.Player;
        player.World.OnUpdate(() => player.CheckProjectileHit(ProjectileId, TargetId, Elapsed, TargetPos));
        //var target = player.World.Entities.Get(TargetId);
        //if (target == null)
        //    return;

        //target.ProjectileHit(player, ProjectileId);
    }

    public void Read(NetworkReader rdr)
    {
        ProjectileId = rdr.ReadInt32();
        TargetId = rdr.ReadInt32();
        Elapsed = rdr.ReadInt32();
        TargetPos = WorldPosData.Read(rdr);
    }
}
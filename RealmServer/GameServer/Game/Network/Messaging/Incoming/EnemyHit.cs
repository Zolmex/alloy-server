#region

using Common;
using Common.Utilities;
using Common.Utilities.Net;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming
{
    [Packet(PacketId.ENEMYHIT)]
    public partial record EnemyHit : IIncomingPacket
    {
        public int ProjectileId;
        public int TargetId;
        public int Elapsed;
        public WorldPosData TargetPos;

        public void Read(NetworkReader rdr)
        {
            ProjectileId = rdr.ReadInt32();
            TargetId = rdr.ReadInt32();
            Elapsed = rdr.ReadInt32();
            TargetPos = WorldPosData.Read(rdr);
        }

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
    }
}
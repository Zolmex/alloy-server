#region

using Common;
using Common.ProjectilePaths;
using Common.Utilities;

#endregion

namespace GameServer.Game.Network.Messaging.Outgoing
{
    [Packet(PacketId.ENEMYSHOOT)]
    public class EnemyShoot : IOutgoingPacket
    {
        public int OwnerId { get; }
        public int ProjId { get; }
        public byte BulletId { get; }
        public WorldPosData StartPosition { get; }
        public float Angle { get; }
        public uint Damage { get; }
        public byte PathType { get; }

        public static void Write(NetworkHandler network, ushort bulletId, int ownerId, ushort projType, float x, float y,
            float angle, short damage,
            byte numShots, float angleInc, ProjectilePath path, float lifetime, bool multiHit, bool passesCover,
            bool armorPiercing, int size, (ConditionEffectIndex, int)[] effects, int propId = -1)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            using (TimedLock.Lock(state))
            {
                var begin = state.PacketBegin();

                wtr.Write(bulletId);
                wtr.Write(ownerId);
                wtr.Write(x);
                wtr.Write(y);
                wtr.Write(angle);
                wtr.Write(damage);
                wtr.Write(propId); // -1 means we're sending custom properties, otherwise use xml properties
                if (propId == -1)
                {
                    // Write custom projectile properties
                    wtr.Write(projType);
                    path.Write(wtr);
                    wtr.Write(lifetime);
                    wtr.Write(multiHit);
                    wtr.Write(passesCover);
                    wtr.Write(armorPiercing);
                    wtr.Write(size);
                    wtr.Write((ushort)(effects?.Length ?? 0));
                    if (effects != null)
                        foreach (var eff in effects)
                        {
                            wtr.Write((ushort)eff.Item1);
                        }
                }

                if (numShots > 1)
                {
                    wtr.Write(numShots);
                    wtr.Write(angleInc);
                }

                state.PacketEnd(begin, PacketId.ENEMYSHOOT);
            }
        }

        public override string ToString()
        {
            var type = typeof(EnemyShoot);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }

            return ret;
        }
    }
}
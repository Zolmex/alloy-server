#region

using Common;
using Common.Network;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Numerics;

#endregion

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERSHOOT)]
public partial record PlayerShoot : IIncomingPacket
{
    public float Angle;
    public float AngleInc;
    public float[] CritList;
    public int[] DamageList;
    public bool IsServerShoot;
    public short ItemType = -1;
    public WorldPosData Pos;
    public int Time;

    public void Handle(User user)
    {
        var player = user.GameInfo.Player;
        if (user.State != ConnectionState.Ready || user.GameInfo.State != GameState.Playing)
            return;

        var weapon = ItemType == -1 ? player.Inventory[0] : new Item(XmlLibrary.ItemDescs[(ushort)ItemType].Root);
        if (weapon == null || weapon.ObjectType == 0)
            return;

        if (weapon.Projectiles[0] == null)
            return;

        if (!IsServerShoot)
        {
            if (!player.ValidateAttackSpeed(weapon, Time))
            {
                player.FailedShoot(weapon.NumProjectiles);

                var attackPeriod = 1 / (player.GetAttackFrequency() * weapon.RateOfFire) * 1000;
                var deltaServer = RealmManager.RealTime.ElapsedMilliseconds - player.LastShootAck;
                var deltaClient = Time - player.LastClientShootTime;

                Console.WriteLine(
                    $"Failed PlayerShoot: attack speed check | period: {attackPeriod}  serverDelta: {deltaServer}  clientDelta:{deltaClient}");
                return;
            }

            // Compare player's position (updated last tick) with projectile position (player's position on the client)
            var moveThresh =
                4 + (player.Speed *
                     (player.TimeSinceLastMove() / 1000f)); // 4 tiles to account for any lag or speed modifiers
            moveThresh *= moveThresh; // Squared
            var dist = player.DistSqr(Pos.X, Pos.Y);
            if (dist > moveThresh)
            {
                player.FailedShoot(weapon
                    .NumProjectiles); // Needed to keep client and server's projectile ids matching
                Console.WriteLine($"PlayerShoot: player position check. Thresh:{moveThresh} Dist:{dist}");
                return;
            }
        }
        else
        {
            if (!player.ValidateServerShoot(ItemType, Pos, Angle, AngleInc, DamageList, CritList))
            {
                player.FailedShoot(DamageList.Length);
                Console.WriteLine($"Failed ServerShoot Item:{ItemType} Count:{DamageList.Length}");
                return;
            }

            player.Shoot(weapon.Projectiles[0], (byte)DamageList.Length, Angle.Deg2Rad(), AngleInc.Deg2Rad(), new Vector2(Pos.X, Pos.Y), DamageList, CritList);
            return;
        }

        player.Shoot(weapon.Projectiles[0], weapon.NumProjectiles, Angle.Deg2Rad(), weapon.ArcGap.Deg2Rad(), new Vector2(Pos.X, Pos.Y));
        player.LastShootAck = RealmManager.RealTime.ElapsedMilliseconds;
        player.LastClientShootTime = Time;
    }

    public void Read(ref SpanReader rdr)
    {
        Angle = rdr.ReadSingle();
        Pos = WorldPosData.Read(ref rdr);
        Time = rdr.ReadInt32();
        IsServerShoot = rdr.ReadBoolean();
        if (IsServerShoot)
        {
            AngleInc = rdr.ReadSingle();
            DamageList = new int[rdr.ReadByte()];
            for (var i = 0; i < DamageList.Length; i++)
                DamageList[i] = rdr.ReadInt32();
            CritList = new float[rdr.ReadByte()];
            for (var i = 0; i < CritList.Length; i++)
                CritList[i] = rdr.ReadSingle();
            ItemType = rdr.ReadInt16();
        }
        else ItemType = -1;
    }
}
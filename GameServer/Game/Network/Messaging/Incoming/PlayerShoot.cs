using System;
using System.Drawing.Imaging;
using System.Numerics;
using Common.Network;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities.Old;
using GameServer.Game.Entities.Old.Extensions;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERSHOOT)]
public record PlayerShoot : IIncomingPacket {
    public float Angle;

    public async Task Handle(User user) {
        if (user.State != ConnectionState.Ready || user.Session.State != GameState.Playing)
            return;

        var player = new EntityView(user.Session.World, user.Session.PlayerId);
        var weapon = player.Inventory[0];
        if (weapon == null || weapon.ObjectType == 0)
            return;

        var projDesc = weapon.Projectiles[0];
        if (projDesc == null)
            return;

        var damage = player.Combat.GetProjectileDamage(projDesc.MinDamage, projDesc.MaxDamage);
        var pos = player.Stats.Pos;
        var world = player.World;
        GameLogic.Enqueue(() => world.SpawnProjectiles(pos, user.Session.PlayerId, Angle.Rad2Deg(), weapon.ArcGap, damage, weapon.NumProjectiles,
            ProjectilePathSegment.ParsePath(projDesc).ToPath(), projDesc.LifetimeMS, projDesc.MultiHit,
            ref GameLogic.WorldTime));
    }

    public void Read(ref SpanReader rdr) {
        Angle = rdr.ReadSingle();
    }
}
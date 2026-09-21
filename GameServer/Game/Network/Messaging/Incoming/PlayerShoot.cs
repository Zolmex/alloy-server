using System;
using System.Drawing.Imaging;
using System.Numerics;
using Common.Network;
using Common.Projectiles.ProjectilePaths;
using Common.Resources.Xml;
using Common.Resources.Xml.Descriptors;
using Common.Structs;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old.Extensions;

namespace GameServer.Game.Network.Messaging.Incoming;

[Packet(PacketId.PLAYERSHOOT)]
public record PlayerShoot : IIncomingPacket {
    public float Angle;

    public async Task Handle(User user) {
        if (user.State != ConnectionState.Ready || user.Session.State != SessionState.Playing)
            return;

        var player = new EntityContext(user.Session.World, user.Session.Player);
        var weapon = player.Inventory[0];
        if (weapon == null || weapon.ObjectType == 0)
            return;

        var projDesc = weapon.Projectiles[0];
        if (projDesc == null)
            return;

        var damage = player.Combat.GetProjectileDamage(projDesc.MinDamage, projDesc.MaxDamage);
        var startAngle = Angle;
        var angleInc = weapon.ArcGap.Deg2Rad();
        for (var i = 0; i < weapon.NumProjectiles; i++) {
            var projData = new ProjectileData() {
                Owner = player.Entity,
                LocalId = player.Combat.GetNextProjectileId(),
                StartPos = player.Position.Pos,
                StartTime = GameLogic.WorldTime.TotalElapsedMs,
                Path = PathSegment.ParsePath(projDesc).ToPath(),
                Angle = startAngle + i * angleInc,
                Damage = damage,
                LifetimeMs = projDesc.LifetimeMS,
                MultiHit = projDesc.MultiHit
            };
            player.World.ProjectileSystem.Create(ref projData);
        }
    }

    public void Read(ref SpanReader rdr) {
        Angle = rdr.ReadSingle();
    }
}
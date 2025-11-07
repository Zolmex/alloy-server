using Common;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;

namespace GameServer.Game.Entities;

public class AssassinPoisonAbility : Ability
{
    private static readonly Logger _log = new(typeof(AssassinPoisonAbility));

    private long _cooldownReset;

    public AssassinPoisonAbility(Player player) : base(player)
    {
    }

    public override void Use(Item item, WorldPosData usePos, int clientTime)
    {
        _cooldownReset = RealmManager.WorldTime.TotalElapsedMs + RealmManager.TicksFromTime(item.Cooldown);

        var dx = usePos.X - _player.Position.X;
        var dy = usePos.Y - _player.Position.Y;
        var distance = (float)Math.Sqrt(dx * dx + dy * dy);

        // Change usePos to not go over max range
        if (distance > item.Poison.ThrowRange)
        {
            var angle = (float)Math.Atan2(dy, dx);
            usePos.X = _player.Position.X + (float)(Math.Cos(angle) * item.Poison.ThrowRange);
            usePos.Y = _player.Position.Y + (float)(Math.Sin(angle) * item.Poison.ThrowRange);
        }

        switch (item.ObjectType)
        {
            default:
                UseTieredPoison(item, usePos, clientTime);
                break;
        }
    }

    public override bool Validate(Item item, Entity en)
    {
        if (item.Poison == null || en != _player || _player.MP < item.Poison.MpCost)
            return false;

        return _cooldownReset <= RealmManager.WorldTime.TotalElapsedMs;
    }

    private void UseTieredPoison(Item item, WorldPosData usePos, int clientTime)
    {
        var poison = item.Poison;

        _player.User.SendPacket(new ShowEffect(
(byte)ShowEffectIndex.Throw,
            _player.Id,
            0,
            poison.ThrowTravelTime,
            new WorldPosData(usePos.X, usePos.Y),
            default);

        // Schedule poison impact after travel time
        RealmManager.AddTimedAction(poison.ThrowTravelTime, () => { PoisonEnemies(item, usePos, RealmManager.WorldTime.TotalElapsedMs); });
    }

    private void PoisonEnemies(Item item, WorldPosData usePos, long time)
    {
        var poison = item.Poison;

        foreach (var enemy in _player.World.GetEnemiesWithin(usePos.X, usePos.Y, poison.PoisonRange))
        {
            if (enemy is not Enemy)
                continue;

            // Create new poison instance for every enemy
            new PoisonInstance(item, enemy, _player, time, 100f);
        }

        _player.User.SendPacket(new ShowEffect(
(byte)ShowEffectIndex.Nova,
            _player.Id,
            0,
            poison.PoisonRange,
            new WorldPosData(usePos.X, usePos.Y),
            new WorldPosData()));
    }
}
using System;
using Common;
using Common.Game;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class HealSelfInfo {
    public int TimeLeft;
}

public record HealSelf : BehaviorScript {
    private readonly int _amount;
    private readonly int _coolDown;

    // New field for the initial one-time cooldown offset
    private readonly int _cooldownOffset;
    private readonly bool _percentage;

    public HealSelf(int coolDown = 0, int amount = 0, bool percentage = false, int cooldownOffset = 0) {
        _coolDown = coolDown;
        _amount = amount;
        _percentage = percentage;
        _cooldownOffset = cooldownOffset;
    }

    public override void Start(ref EntityView host) {
        var healGroupInfo = host.Behavior.Resources.ResolveResource<HealSelfInfo>(this);
        // Instead of forcing TimeLeft = 0, start it at the cooldownOffset.
        healGroupInfo.TimeLeft = _cooldownOffset;
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var healSelfInfo = host.Behavior.Resources.ResolveResource<HealSelfInfo>(this);

        // If we still have time left in the (offset or cooldown) timer, decrement and remain on cooldown
        if (healSelfInfo.TimeLeft > 0) {
            healSelfInfo.TimeLeft -= time.ElapsedMsDelta;
            return BehaviorTickState.OnCooldown;
        }

        // If the host is stunned, do not heal
        // if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) // TODO: COndition effects
        //     return BehaviorTickState.BehaviorFailed;

        // Calculate the potential heal amount
        var maxHp = host.Stats.GetInt(StatType.MaxHP);
        var hp = host.Stats.GetInt(StatType.HP);
        var healValue = 0;
        if (_amount != 0) {
            healValue = _amount;
            if (_percentage) healValue = (int)(_amount * maxHp / 100.0);
        }

        // Calculate the actual heal amount (capped by MaxHP)
        var actualHeal = Math.Min(healValue, maxHp - hp);

        // Apply the heal if there's a positive difference
        if (actualHeal > 0) {
            host.Stats.Set(StatType.HP, hp + actualHeal);
            hp += actualHeal;

            // Ensure HP doesn't exceed MaxHP
            if (hp > maxHp)
                host.Stats.Set(StatType.HP, maxHp);

            // Show effect broadcasts
            var hostId = host.Id;
            var hostPos = host.Stats.Pos;
            host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user => {
                user.SendPacket(new ShowEffect(
                    (byte)ShowEffectIndex.Heal,
                    hostId,
                    0xFFFFFF,
                    0,
                    default,
                    default));
            });
            host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user => {
                user.SendPacket(new ShowEffect(
                    (byte)ShowEffectIndex.Line,
                    hostId,
                    0xFFFFFF,
                    0,
                    hostPos,
                    default));
            });
            host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user => {
                user.SendPacket(new Notification(
                    hostId,
                    "+" + actualHeal, // Display the actual health restored
                    0x00FF00));
            });
        }

        // Set the normal cooldown for subsequent ticks
        healSelfInfo.TimeLeft = _coolDown;

        return BehaviorTickState.BehaviorActive;
    }
}
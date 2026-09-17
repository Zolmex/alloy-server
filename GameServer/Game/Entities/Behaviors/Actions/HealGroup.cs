using Common;
using Common.Game;
using Common.Utilities.Collections;
using GameServer.Game.Entities.Components;
using GameServer.Game.Entities.Old;
using GameServer.Game.Network.Messaging.Outgoing;

namespace GameServer.Game.Entities.Behaviors.Actions;

public class HealGroupInfo {
    public int RemainingTime;
}

public record HealGroup : BehaviorScript {
    private readonly int _cooldownMS;
    private readonly string _group;
    private readonly int _healAmount;
    private readonly float _range;

    public HealGroup(float range, string group, int cooldownMS = 1000, int healAmount = 0) {
        _range = range;
        _group = group;
        _cooldownMS = cooldownMS;
        _healAmount = healAmount;
    }

    public override void Start(ref EntityContext host) {
        var healGroupInfo = host.BehavController.Resources.ResolveResource<HealGroupInfo>(this);
        healGroupInfo.RemainingTime = 0; // Make sure the behavior runs once
    }

    public override BehaviorTickState Tick(ref EntityContext host, ref RealmTime time) {
        var healGroupInfo = host.BehavController.Resources.ResolveResource<HealGroupInfo>(this);
        if (healGroupInfo.RemainingTime <= 0) {
            // if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) // TODO: Condition Effects
            //     return BehaviorTickState.BehaviorFailed;

            foreach (var en in host.World.Map.GetEntitiesByName(host.Position.Pos, _group, _range)) {
                ref var stats = ref host.World.Ecs.Get<Stats>(en);
                var newHp = stats.GetInt(StatType.MaxHP);
                var hp = stats.GetInt(StatType.HP);
                if (_healAmount != 0) {
                    var newHealth = _healAmount + hp;
                    if (newHp > newHealth)
                        newHp = newHealth;
                }

                var hostId = (EntityId)host.Entity;
                var hostPos = host.Position.Pos;
                if (newHp != hp) {
                    var n = newHp - hp;

                    stats.Set(StatType.HP, newHp);
                    host.World.Map.BroadcastNearby(hostPos, 20f, user =>
                        user.SendPacket(new ShowEffect(
                            (byte)ShowEffectIndex.Heal,
                            (EntityId)en,
                            0xFFFFFF,
                            0,
                            default,
                            default
                        )));
                    host.World.Map.BroadcastNearby(hostPos, 20f, user =>
                        user.SendPacket(new ShowEffect(
                            (byte)ShowEffectIndex.Line,
                            hostId,
                            0xFFFFFF,
                            0,
                            hostPos,
                            default
                        )));
                    host.World.Map.BroadcastNearby(hostPos, 20f, user =>
                        user.SendPacket(new Notification(
                            (EntityId)en,
                            "+" + n,
                            0x00FF00)
                        ));
                }
            }

            healGroupInfo.RemainingTime = _cooldownMS;
        }
        else {
            healGroupInfo.RemainingTime -= time.ElapsedMsDelta;
        }

        return BehaviorTickState.BehaviorActive;
    }
}
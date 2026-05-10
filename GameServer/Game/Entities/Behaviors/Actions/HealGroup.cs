using Common;
using Common.Game;
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

    public override void Start(ref EntityView host) {
        var healGroupInfo = host.Behavior.Resources.ResolveResource<HealGroupInfo>(this);
        healGroupInfo.RemainingTime = 0; // Make sure the behavior runs once
    }

    public override BehaviorTickState Tick(ref EntityView host, ref RealmTime time) {
        var healGroupInfo = host.Behavior.Resources.ResolveResource<HealGroupInfo>(this);
        if (healGroupInfo.RemainingTime <= 0) {
            // if (host.HasConditionEffect(ConditionEffectIndex.Stunned)) // TODO: Condition Effects
            //     return BehaviorTickState.BehaviorFailed;

            foreach (var enId in host.World.Map.GetEntitiesByName(host.Stats.Pos, _group, _range)) {
                ref var stats = ref host.World.EntityStats.Get(enId);
                var newHp = stats.GetInt(StatType.MaxHP);
                var hp = stats.GetInt(StatType.HP);
                if (_healAmount != 0) {
                    var newHealth = _healAmount + hp;
                    if (newHp > newHealth)
                        newHp = newHealth;
                }

                var hostId = host.Id;
                var hostPos = host.Stats.Pos;
                if (newHp != hp) {
                    var n = newHp - hp;

                    stats.Set(StatType.HP, newHp);
                    host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user =>
                        user.SendPacket(new ShowEffect(
                            (byte)ShowEffectIndex.Heal,
                            enId,
                            0xFFFFFF,
                            0,
                            default,
                            default
                        )));
                    host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user =>
                        user.SendPacket(new ShowEffect(
                            (byte)ShowEffectIndex.Line,
                            hostId,
                            0xFFFFFF,
                            0,
                            hostPos,
                            default
                        )));
                    host.World.Map.BroadcastNearby(host.Stats.Pos, 20f, user =>
                        user.SendPacket(new Notification(
                            enId,
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
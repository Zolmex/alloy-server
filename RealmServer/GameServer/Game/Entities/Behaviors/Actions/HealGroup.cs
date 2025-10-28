#region

using Common;
using GameServer.Game.Network.Messaging.Outgoing;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class HealGroupInfo
    {
        public int RemainingTime;
    }

    public record HealGroup : BehaviorScript
    {
        private readonly float _range;
        private readonly string _group;
        private readonly int _cooldownMS;
        private readonly int _healAmount;

        public HealGroup(float range, string group, int cooldownMS = 1000, int healAmount = 0)
        {
            _range = range;
            _group = group;
            _cooldownMS = cooldownMS;
            _healAmount = healAmount;
        }

        public override void Start(Character host)
        {
            var healGroupInfo = host.ResolveResource<HealGroupInfo>(this);
            healGroupInfo.RemainingTime = 0; // Make sure the behavior runs once
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var healGroupInfo = host.ResolveResource<HealGroupInfo>(this);
            if (healGroupInfo.RemainingTime <= 0)
            {
                if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                    return BehaviorTickState.BehaviorFailed;

                foreach (var entity in host.GetOtherEnemiesByName(_group, _range))
                {
                    if (entity is not Character character)
                        continue;

                    var newHp = entity.MaxHP;
                    if (_healAmount != 0)
                    {
                        var newHealth = _healAmount + entity.HP;
                        if (newHp > newHealth)
                            newHp = newHealth;
                    }

                    if (newHp != entity.HP)
                    {
                        var n = newHp - entity.HP;

                        entity.HP = newHp;
                        entity.World.BroadcastAll(p => ShowEffect.Write(p.User.Network,
                            (byte)ShowEffectIndex.Heal,
                            entity.Id,
                            0xFFFFFF,
                            0,
                            default,
                            default
                        ));
                        entity.World.BroadcastAll(p => ShowEffect.Write(p.User.Network,
                            (byte)ShowEffectIndex.Line,
                            host.Id,
                            0xFFFFFF,
                            0,
                            entity.Position,
                            default
                        ));
                        entity.World.BroadcastAll(p =>
                            Notification.Write(p.User.Network,
                                entity.Id,
                                "+" + n,
                                0x00FF00)
                        );
                    }
                }

                healGroupInfo.RemainingTime = _cooldownMS;
            }
            else
                healGroupInfo.RemainingTime -= time.ElapsedMsDelta;

            return BehaviorTickState.BehaviorActive;
        }
    }
}
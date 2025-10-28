#region

using Common;
using GameServer.Game.Network.Messaging.Outgoing;
using System;

#endregion

namespace GameServer.Game.Entities.Behaviors.Actions
{
    public class HealSelfInfo
    {
        public int TimeLeft;
    }

    public record HealSelf : BehaviorScript
    {
        private readonly int _amount;
        private readonly bool _percentage;
        private readonly int _coolDown;

        // New field for the initial one-time cooldown offset
        private readonly int _cooldownOffset;

        /// <summary>
        /// Heals the host after an optional initial offset, then repeats based on the specified cooldown.
        /// </summary>
        /// <param name="coolDown">Cooldown between heals after the first heal fires.</param>
        /// <param name="amount">Amount to heal (absolute or percentage).</param>
        /// <param name="percentage">If true, treat amount as a percentage [0..100].</param>
        /// <param name="cooldownOffset">An initial one-time delay before the first heal triggers.</param>
        public HealSelf(int coolDown = 0, int amount = 0, bool percentage = false, int cooldownOffset = 0)
        {
            _coolDown = coolDown;
            _amount = amount;
            _percentage = percentage;
            _cooldownOffset = cooldownOffset;
        }

        public override void Start(Character host)
        {
            var healGroupInfo = host.ResolveResource<HealSelfInfo>(this);
            // Instead of forcing TimeLeft = 0, start it at the cooldownOffset.
            healGroupInfo.TimeLeft = _cooldownOffset;
        }

        public override BehaviorTickState Tick(Character host, RealmTime time)
        {
            var healSelfInfo = host.ResolveResource<HealSelfInfo>(this);

            // If we still have time left in the (offset or cooldown) timer, decrement and remain on cooldown
            if (healSelfInfo.TimeLeft > 0)
            {
                healSelfInfo.TimeLeft -= time.ElapsedMsDelta;
                return BehaviorTickState.OnCooldown;
            }

            // If the host is stunned, do not heal
            if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                return BehaviorTickState.BehaviorFailed;

            // Calculate the potential heal amount
            int healValue = 0;
            if (_amount != 0)
            {
                healValue = _amount;
                if (_percentage)
                {
                    healValue = (int)(_amount * host.MaxHP / 100.0);
                }
            }

            // Calculate the actual heal amount (capped by MaxHP)
            int currentHp = host.HP;
            int maxHp = host.MaxHP;
            int actualHeal = Math.Min(healValue, maxHp - currentHp);

            // Apply the heal if there's a positive difference
            if (actualHeal > 0)
            {
                host.HP += actualHeal;

                // Ensure HP doesn't exceed MaxHP
                if (host.HP > maxHp)
                    host.HP = maxHp;

                // Show effect broadcasts
                host.World.BroadcastAll(p =>
                {
                    if (p.DistSqr(host) <= Player.SIGHT_RADIUS_SQR)
                        ShowEffect.Write(p.User.Network,
                            (byte)ShowEffectIndex.Heal,
                            host.Id,
                            0xFFFFFF,
                            0,
                            default,
                            default);
                });
                host.World.BroadcastAll(p =>
                {
                    if (p.DistSqr(host) <= Player.SIGHT_RADIUS_SQR)
                        ShowEffect.Write(p.User.Network,
                            (byte)ShowEffectIndex.Line,
                            host.Id,
                            0xFFFFFF,
                            0,
                            host.Position,
                            default);
                });
                host.World.BroadcastAll(p =>
                {
                    if (p.DistSqr(host) <= Player.SIGHT_RADIUS_SQR)
                        Notification.Write(p.User.Network,
                            host.Id,
                            "+" + actualHeal, // Display the actual health restored
                            0x00FF00);
                });
            }

            // Set the normal cooldown for subsequent ticks
            healSelfInfo.TimeLeft = _coolDown;

            return BehaviorTickState.BehaviorActive;
        }

    }
}
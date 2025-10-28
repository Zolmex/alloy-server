#region

using Common;
using Common.ProjectilePaths;
using Common.Resources.Config;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using GameServer.Game.Entities.Behaviors.Actions;
using GameServer.Game.Entities.Behaviors.Transitions;
using System;
using System.Linq;
using static GameServer.Game.Entities.Behaviors.BehaviorScript;

#endregion

namespace GameServer.Game.Entities.Behaviors
{
    public class PaladinBanner : EntityBehavior
    {
        private static readonly Logger _log = new Logger(typeof(PaladinBanner));
        public SealDesc Seal;
        public Player Player;
        private bool lifetimeSet = false;
        private Follow Follow;
        private long nextBuffTime;

        public override void RegisterStates()
        {
            StateManager.RegisterState(PaladinBannerState.Buff, BuffTick);
        }

        public override void RegisterBehaviors()
        {
        }

        public override void Initialize(Character owner)
        {
            StateManager.SetCurrentState(owner, PaladinBannerState.Buff);
            base.Initialize(owner);
        }
        
        private void BuffTick(RealmTime time, Character owner, StateTick state)
        {
            if (Seal == null)
                return;
            if (!lifetimeSet)
            {
                owner.Lifetime = Seal.Duration;
                lifetimeSet = true;
                Follow = new Follow(Player.GetSpeed(Player.MovementSpeed) * (Seal.BannerSpeed / 100), 1.5f, 15, target: Player.Name);
                nextBuffTime = time.TotalElapsedMs;
            }

            Follow.Tick(owner, time);

            if (nextBuffTime > time.TotalElapsedMs)
                return;
            nextBuffTime += Seal.BoostDuration;

            var playerCount = 0;
            var playerWithin = owner.GetPlayersWithin(Seal.Radius);
            var enumerable = playerWithin.ToList();
            var stack = enumerable.Count > Seal.MaxStack ? Seal.MaxStack : enumerable.Count;
            foreach (var player in enumerable)
            {
                playerCount++;
                foreach (var modifier in Seal.StatsModifier)
                {
                    var stat = Enum.Parse<StatType>(modifier.Stat);
                    if (modifier.BoostType == "Percentage")
                        player.AddIncreasedBonus(stat, modifier.Amount * stack);
                    else if (modifier.BoostType == "Static")
                        player.AddFlatBonus(stat, modifier.Amount * stack);
                }

                if (playerCount == Seal.MaxAllies)
                    break;
            }
            
            owner.World.AddTimedAction(Seal.BoostDuration, () =>
            {
                playerCount = 0;
                foreach (var player in enumerable)
                {
                    playerCount++;
                    if (player.World != owner.World)
                        continue;
                    foreach (var modifier in Seal.StatsModifier)
                    {
                        var stat = Enum.Parse<StatType>(modifier.Stat);
                        if (modifier.BoostType == "Percentage")
                            player.RemoveIncreasedBonus(stat, modifier.Amount * stack);
                        else if (modifier.BoostType == "Static")
                            player.RemoveFlatBonus(stat, modifier.Amount * stack);
                    }

                    if (playerCount == Seal.MaxAllies)
                        break;
                }
            });
        }
        
        public enum PaladinBannerState
        {
            Buff,
        }
    }
}